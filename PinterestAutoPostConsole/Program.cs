using PinterestAutoPostConsole;

var auth = new SaveSession();
var driver = auth.SaveAndGetSession();
PinterestAutoPoster pin = new PinterestAutoPoster(driver);
pin.PostPins();