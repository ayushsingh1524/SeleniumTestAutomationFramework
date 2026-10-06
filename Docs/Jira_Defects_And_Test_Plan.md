# SauceDemo E2E Testing - Documentation

## 1. Test Plan Overview
This test plan covers the regression testing of the Swag Labs (SauceDemo) e-commerce web application. The testing focuses on End-to-End user flows, including authentication, inventory management, cart operations, and the checkout process.

**Scope:**
- UI & Functional Testing
- Cross-browser capability (Chrome, Edge, Firefox)
- Performance & Edge cases (Glitch user, Error user)

**Strategy:**
- **Automated Testing:** 18 test scripts in C# using NUnit and Page Object Model, plus an 8-test parallel suite in JavaScript using Mocha.
- **CI/CD:** Both test suites are integrated into GitHub Actions, executing on every push.

---

## 2. Jira Defect Log

| Defect ID | Title | Severity | Reproduction Steps | Expected Result | Actual Result | Status |
| :---: | :--- | :---: | :--- | :--- | :--- | :---: |
| BUG-101 | "Problem User" sees broken images on Inventory Page | High | 1. Login as `problem_user`<br>2. Navigate to Inventory | All product images should render correctly. | Images fail to load, showing a default placeholder. | Open |
| BUG-102 | "Error User" cannot complete checkout | Critical | 1. Login as `error_user`<br>2. Add item to cart<br>3. Proceed to checkout<br>4. Click 'Finish' | User should be redirected to Checkout Complete page. | Clicking 'Finish' does nothing. | Open |
| BUG-103 | "Performance Glitch User" login exceeds 3 seconds | Medium | 1. Login as `performance_glitch_user`<br>2. Measure time to Inventory page | Login should complete in < 1 second. | Login takes ~5 seconds to complete. | Open |
| BUG-104 | "Problem User" cannot sort items properly | High | 1. Login as `problem_user`<br>2. Change sort order to "Price (low to high)" | Items should be sorted by price ascending. | Sort order remains unchanged or becomes erratic. | Open |
| BUG-105 | "Error User" missing Last Name input functionality | High | 1. Login as `error_user`<br>2. Go to checkout step 1<br>3. Try typing in Last Name field | Last Name field should accept text input. | Last Name field clears immediately or ignores input. | Open |

---

## 3. Regression Coverage (30 Test Cases)

1. **TC_Auth_01:** Verify standard user can login successfully.
2. **TC_Auth_02:** Verify locked out user cannot login.
3. **TC_Auth_03:** Verify empty username displays correct error.
4. **TC_Auth_04:** Verify empty password displays correct error.
5. **TC_Auth_05:** Verify invalid credentials display correct error.
6. **TC_Auth_06:** Verify standard user can logout.
7. **TC_Inv_07:** Verify all 6 products load on Inventory page.
8. **TC_Inv_08:** Verify product images are not broken for standard user.
9. **TC_Inv_09:** Verify sorting by Name (A to Z).
10. **TC_Inv_10:** Verify sorting by Name (Z to A).
11. **TC_Inv_11:** Verify sorting by Price (Low to High).
12. **TC_Inv_12:** Verify sorting by Price (High to Low).
13. **TC_Inv_13:** Verify product name links to details page.
14. **TC_Inv_14:** Verify product image links to details page.
15. **TC_Cart_15:** Verify adding a single item updates cart badge to '1'.
16. **TC_Cart_16:** Verify adding multiple items updates cart badge correctly.
17. **TC_Cart_17:** Verify 'Add to Cart' button changes to 'Remove'.
18. **TC_Cart_18:** Verify 'Remove' button correctly removes item and updates badge.
19. **TC_Cart_19:** Verify cart page lists correct items added from inventory.
20. **TC_Cart_20:** Verify removing item from Cart page works.
21. **TC_Cart_21:** Verify 'Continue Shopping' button navigates back to inventory.
22. **TC_Chk_22:** Verify 'Checkout' button navigates to step one.
23. **TC_Chk_23:** Verify checkout requires First Name.
24. **TC_Chk_24:** Verify checkout requires Last Name.
25. **TC_Chk_25:** Verify checkout requires Zip/Postal Code.
26. **TC_Chk_26:** Verify checkout overview calculates item total correctly.
27. **TC_Chk_27:** Verify checkout overview calculates tax correctly.
28. **TC_Chk_28:** Verify checkout overview calculates final total correctly.
29. **TC_Chk_29:** Verify 'Finish' button navigates to complete page.
30. **TC_Chk_30:** Verify 'Back Home' button on complete page returns to inventory.
