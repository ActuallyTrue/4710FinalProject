//Trigger is a struct that has the property Value in it. Properties are glorified getters and setters that you can just add to a variable.
//When Value is called, if the Value of Value is needed, C# will run the code in get. If we're setting value, C# will run the code in set.
public struct Trigger {
    private bool val;
    public bool Value
    {
        get
        {
            bool old = val;
            val = false;
            return old;
        }
        set
        {
            val = value;
        }
    }
}
