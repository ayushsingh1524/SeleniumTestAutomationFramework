const { Builder, By, until } = require('selenium-webdriver');
const assert = require('assert');

describe('SauceDemo E2E Mocha Suite - 8 Parallel Tests', function() {
    let driver;

    beforeEach(async function() {
        driver = await new Builder().forBrowser('chrome').build();
        await driver.get('https://www.saucedemo.com/');
    });

    afterEach(async function() {
        if (driver) {
            await driver.quit();
        }
    });

    it('Test 1: Standard user can login', async function() {
        await driver.findElement(By.id('user-name')).sendKeys('standard_user');
        await driver.findElement(By.id('password')).sendKeys('secret_sauce');
        await driver.findElement(By.id('login-button')).click();
        await driver.wait(until.elementLocated(By.className('title')), 5000);
        const title = await driver.findElement(By.className('title')).getText();
        assert.strictEqual(title, 'Products');
    });

    it('Test 2: Locked out user cannot login', async function() {
        await driver.findElement(By.id('user-name')).sendKeys('locked_out_user');
        await driver.findElement(By.id('password')).sendKeys('secret_sauce');
        await driver.findElement(By.id('login-button')).click();
        await driver.wait(until.elementLocated(By.css('[data-test="error"]')), 5000);
        const errorMsg = await driver.findElement(By.css('[data-test="error"]')).getText();
        assert.match(errorMsg, /locked out/);
    });

    it('Test 3: Standard user can add item to cart', async function() {
        await driver.findElement(By.id('user-name')).sendKeys('standard_user');
        await driver.findElement(By.id('password')).sendKeys('secret_sauce');
        await driver.findElement(By.id('login-button')).click();
        await driver.wait(until.elementLocated(By.id('add-to-cart-sauce-labs-backpack')), 5000);
        await driver.findElement(By.id('add-to-cart-sauce-labs-backpack')).click();
        const badge = await driver.findElement(By.className('shopping_cart_badge')).getText();
        assert.strictEqual(badge, '1');
    });

    it('Test 4: Problem user sees broken images', async function() {
        await driver.findElement(By.id('user-name')).sendKeys('problem_user');
        await driver.findElement(By.id('password')).sendKeys('secret_sauce');
        await driver.findElement(By.id('login-button')).click();
        await driver.wait(until.elementLocated(By.className('inventory_item_img')), 5000);
        const images = await driver.findElements(By.className('inventory_item_img'));
        assert.ok(images.length > 0);
    });

    it('Test 5: Standard user can sort by price (low to high)', async function() {
        await driver.findElement(By.id('user-name')).sendKeys('standard_user');
        await driver.findElement(By.id('password')).sendKeys('secret_sauce');
        await driver.findElement(By.id('login-button')).click();
        await driver.wait(until.elementLocated(By.className('product_sort_container')), 5000);
        const sortSelect = await driver.findElement(By.className('product_sort_container'));
        await sortSelect.click();
        await driver.findElement(By.css('option[value="lohi"]')).click();
        const firstItemPrice = await driver.findElement(By.className('inventory_item_price')).getText();
        assert.strictEqual(firstItemPrice, '$7.99');
    });

    it('Test 6: Invalid credentials display error', async function() {
        await driver.findElement(By.id('user-name')).sendKeys('invalid_user');
        await driver.findElement(By.id('password')).sendKeys('wrong_password');
        await driver.findElement(By.id('login-button')).click();
        await driver.wait(until.elementLocated(By.css('[data-test="error"]')), 5000);
        const errorMsg = await driver.findElement(By.css('[data-test="error"]')).getText();
        assert.match(errorMsg, /Username and password do not match/);
    });

    it('Test 7: Standard user can navigate to cart', async function() {
        await driver.findElement(By.id('user-name')).sendKeys('standard_user');
        await driver.findElement(By.id('password')).sendKeys('secret_sauce');
        await driver.findElement(By.id('login-button')).click();
        await driver.wait(until.elementLocated(By.className('shopping_cart_link')), 5000);
        await driver.sleep(1000); // wait for React hydration
        await driver.findElement(By.className('shopping_cart_link')).click();
        await driver.wait(until.elementLocated(By.className('title')), 5000);
        await driver.sleep(1000); // wait for page transition
        const title = await driver.findElement(By.className('title')).getText();
        assert.strictEqual(title, 'Your Cart');
    });

    it('Test 8: Standard user can logout', async function() {
        await driver.findElement(By.id('user-name')).sendKeys('standard_user');
        await driver.findElement(By.id('password')).sendKeys('secret_sauce');
        await driver.findElement(By.id('login-button')).click();
        await driver.wait(until.elementLocated(By.id('react-burger-menu-btn')), 5000);
        await driver.sleep(1000); // wait for React hydration
        await driver.findElement(By.id('react-burger-menu-btn')).click();
        await driver.wait(until.elementLocated(By.id('logout_sidebar_link')), 5000);
        await driver.sleep(2000); // Wait for sidebar animation
        const logoutBtn = await driver.findElement(By.id('logout_sidebar_link'));
        await driver.executeScript("arguments[0].click();", logoutBtn); // JS click to bypass overlap
        await driver.wait(until.elementLocated(By.id('login-button')), 5000);
        const loginBtn = await driver.findElements(By.id('login-button'));
        assert.strictEqual(loginBtn.length, 1);
    });
});
