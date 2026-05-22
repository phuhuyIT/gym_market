import time
from playwright.sync_api import sync_playwright

def run():
    with sync_playwright() as p:
        # Launch Chromium headless
        browser = p.chromium.launch(headless=True)
        page = browser.new_page()

        # Capture console messages
        def handle_console(msg):
            print(f"[Console] {msg.type}: {msg.text}")
        page.on("console", handle_console)

        # Capture page errors
        def handle_pageerror(err):
            print(f"[Page Error] {err}")
        page.on("pageerror", handle_pageerror)

        # Listen to API requests/responses
        def handle_response(response):
            if "api/" in response.url:
                print(f"[API Response] {response.status} {response.url}")
                try:
                    # Print json response if available
                    print(f"       Body: {response.text()[:200]}")
                except Exception:
                    pass
        page.on("response", handle_response)

        # Go to login page
        print("Navigating to login page...")
        page.goto("http://localhost:4200/login")
        page.wait_for_load_state("networkidle")

        # Fill login form
        print("Filling credentials...")
        page.fill("input[type='email']", "Bertha_Zemlak@gmail.com")
        page.fill("input[type='password']", "abcdef@123")
        
        # Click login button
        print("Clicking login...")
        page.click("gm-button button")
        
        # Wait for navigation/dashboard load
        time.sleep(3)
        page.wait_for_load_state("networkidle")
        print(f"Logged in. Current URL: {page.url}")

        # Navigate to profile
        print("Navigating to student profile...")
        page.goto("http://localhost:4200/client/your-profile")
        time.sleep(3)
        page.wait_for_load_state("networkidle")
        print(f"Profile URL: {page.url}")

        # Capture DOM structure
        print("Saving screenshot...")
        page.screenshot(path="/workspaces/gym_market/scratch/screenshot.png", full_page=True)

        print("Page HTML:")
        print(page.content()[:1000])

        browser.close()

if __name__ == "__main__":
    run()
