export interface BodyFatMeasurements {
  Age: number;
  Weight: number;
  Height: number;
  Neck: number;
  Chest: number;
  Abdomen: number;
  Thigh: number;
  Knee: number;
  Biceps: number;
}

export interface BodyFatPredictionResponse {
  'Predicted Body Fat Percentage'?: number;
  'Predicted Label'?: number;
  bodyfat?: number;
}
