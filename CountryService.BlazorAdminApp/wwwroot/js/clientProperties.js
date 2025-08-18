
/** Gets some client properties. */
function getClientProperties() {
    
    return "userAgent: " + window.navigator.userAgent + 
        ", screenHeight: " + window.screen.height + 
        ", screenWidth: " + window.screen.width +
        ", cookieEnabled: " + window.navigator.cookieEnabled;
}
