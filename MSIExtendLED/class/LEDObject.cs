using System;

public class LEDObject
{
    public string[] cells;
    public LEDObject(string[] setting)
    {
        cells = new string[] { setting };
    }
    public getDark()
    {
        return cells[9];
    }
    public getHue()
    {
        return cells[8];
    }
    public setDark(string input)
    {
        cells[9] = input;
    }
    public setHue(string input)
    {
        cells[8] = input;
    }
}
