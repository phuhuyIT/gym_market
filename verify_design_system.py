from playwright.sync_api import sync_playwright
import os
import time

def verify_design_system():
    with sync_playwright() as p:
        browser = p.chromium.launch(headless=True)
        page = browser.new_page()
        
        print("Navigating to login page...")
        page.goto('http://localhost:4200/login')
        
        # Wait for the page to load
        page.wait_for_load_state('networkidle')
        
        # Check for CSS variables (Design Tokens)
        print("Checking Design Tokens...")
        bg_color = page.evaluate("getComputedStyle(document.body).backgroundColor")
        print(f"Body background color: {bg_color}")
        
        # Take a screenshot of the login page
        print("Capturing login page screenshot...")
        page.screenshot(path='login_page_verify.png', full_page=True)
        
        # Attempt a failed login to trigger toast
        print("Testing Toast Service (Failed Login)...")
        # Assuming there's an email and password field.
        # Based on login.component.html (before redesign, but I'll try to find inputs)
        inputs = page.locator('input').all()
        if len(inputs) >= 2:
            inputs[0].fill('invalid@example.com')
            inputs[1].fill('wrongpassword')
            
            # Find and click sign in button
            # I replaced the button with app-login's button logic in the .ts, 
            # but the .html is still the old one until Phase 5.
            # Let's find a button.
            button = page.locator('button').first()
            if button:
                button.click()
                
                # Wait for toast to appear
                try:
                    page.wait_for_selector('.gm-toast', timeout=5000)
                    print("SUCCESS: GmToast appeared after failed login.")
                    page.screenshot(path='toast_verification.png')
                except:
                    print("FAILED: GmToast did not appear.")
        else:
            print(f"FAILED: Found {len(inputs)} inputs, expected at least 2.")

        browser.close()

if __name__ == "__main__":
    verify_design_system()
