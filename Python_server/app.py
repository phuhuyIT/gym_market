from fastapi import FastAPI, File, UploadFile
from fastapi.responses import JSONResponse
from pydantic import BaseModel
import joblib
import tensorflow as tf
import numpy as np
from PIL import Image
import cv2
import logging
from fastapi.middleware.cors import CORSMiddleware

# Initialize logging
logging.basicConfig(level=logging.INFO)

# Create the FastAPI app
app = FastAPI()

# Add CORS middleware
origins = [
    "http://localhost",  # Cho phép các domain cụ thể
    "http://localhost:3000",  # Ví dụ: React frontend chạy trên cổng 3000
    "*",  # Hoặc sử dụng '*' để cho phép tất cả (không khuyến khích trong môi trường sản xuất)
]
app.add_middleware(
    CORSMiddleware,
    allow_origins=origins,
    allow_credentials=True,
    allow_methods=["*"],  # Cho phép tất cả các phương thức HTTP (GET, POST, PUT, DELETE, ...)
    allow_headers=["*"],  # Cho phép tất cả các header
)

# ----------------- Shared Utilities -----------------

def preprocess_image(image, target_size=(224, 224)):
    """Preprocess a single image: resize, convert to RGB, and normalize."""
    try:
        resized_image = cv2.resize(image, target_size)
        resized_image = cv2.cvtColor(resized_image, cv2.COLOR_BGR2RGB)
        normalized_image = resized_image.astype('float32') / 255.0
        return normalized_image
    except Exception as e:
        raise ValueError(f"Error in preprocessing image: {e}")

def softmax_to_regression(softmax_output, class_values):
    """Convert softmax probabilities to regression output."""
    return np.dot(softmax_output, class_values)

def load_model_with_custom_objects(model_path, custom_objects=None):
    """Load a model with custom objects if required."""
    try:
        return tf.keras.models.load_model(model_path, custom_objects=custom_objects)
    except Exception as e:
        raise ValueError(f"Error loading model: {e}")

async def predict_image(file: UploadFile, model, class_values, target_size=(224, 224)):
    """Shared prediction logic for image-based endpoints."""
    try:
        if file.content_type not in ["image/jpeg", "image/png"]:
            raise ValueError("Unsupported file type. Only JPEG and PNG are allowed.")

        # Load and preprocess the image
        image = Image.open(file.file)
        image = np.array(image)
        image = preprocess_image(image, target_size)
        image = np.expand_dims(image, axis=0)  # Add batch dimension
        # Make prediction
        prediction = model.predict(image)
        logging.info(f"Prediction: {prediction}")
        regression_values = softmax_to_regression(prediction, class_values)
        return {"Predicted Label": round(float(regression_values[0]), 2)}
    except Exception as e:
        return JSONResponse(content={"error": str(e)}, status_code=500)

# ----------------- Endpoint 1: Body Fat Prediction -----------------

# Load the scaler and body fat prediction model
logging.info("Loading body fat model and scaler...")
scaler = joblib.load("scaler.pkl")
bodyfat_model = load_model_with_custom_objects("bodyfat_model.h5", custom_objects={"mse": tf.keras.losses.MeanSquaredError()})

# Define the input schema for body fat prediction
class InputData(BaseModel):
    Age: float
    Weight: float
    Height: float
    Neck: float
    Chest: float
    Abdomen: float
    Thigh: float
    Knee: float
    Biceps: float

@app.post("/predict_bodyfat/")
async def predict_bodyfat(input_data: InputData):
    try:
        # Convert input data to numpy array
        input_dict = input_data.dict()
        input_array = np.array([list(input_dict.values())]).reshape(1, -1)

        # Scale the input data
        input_scaled = scaler.transform(input_array)

        # Predict the body fat percentage
        prediction = bodyfat_model.predict(input_scaled)

        # Return the result
        return {"Predicted Body Fat Percentage": float(prediction[0][0])}
    except Exception as e:
        return JSONResponse(content={"error": str(e)}, status_code=500)

# ----------------- Endpoint 2: Predict Image Male -----------------

# Load the MobileNet model for males
logging.info("Loading MobileNet model for males...")
mobilenet_model_male = load_model_with_custom_objects("mobilenetv4_model.h5")

# Define regression class values for males
class_values_male = np.array([14, 20, 35, 3, 45])

@app.post("/predict_image_male/")
async def predict_image_male(file: UploadFile = File(...)):
    return await predict_image(file, mobilenet_model_male, class_values_male)

# ----------------- Endpoint 3: Predict Image Female -----------------

# Load the MobileNet model for females
logging.info("Loading MobileNet model for females...")
mobilenet_model_female = load_model_with_custom_objects("mobilenetv4_model_female.h5")

# Define regression class values for females
class_values_female = np.array([8, 19, 35, 50])

@app.post("/predict_image_female/")
async def predict_image_female(file: UploadFile = File(...)):
    try:
        # Validate input file type
        if file.content_type not in ["image/jpeg", "image/png"]:
            raise ValueError("Unsupported file type. Only JPEG and PNG are allowed.")

        # Read the file as an image
        image = await file.read()
        image = np.frombuffer(image, np.uint8)
        image = cv2.imdecode(image, cv2.IMREAD_COLOR)

        # Preprocess the image
        processed_image = preprocess_image(image, target_size=(224, 224))
        processed_image = np.expand_dims(processed_image, axis=0)  # Add batch dimension

        # Predict using the model
        prediction = mobilenet_model_female.predict(processed_image)
        regression_values = softmax_to_regression(prediction, class_values_female)

        # Return the result
        return {"Predicted Label": round(float(regression_values[0]), 2)}
    except Exception as e:
        return JSONResponse(content={"error": str(e)}, status_code=500)
