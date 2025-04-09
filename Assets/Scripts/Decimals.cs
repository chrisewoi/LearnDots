using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
public static class Decimals
{
    public static char[] unitChars = {'K','M','G','T','P','E','Z'};
    public static FixedString32Bytes ToUnitString(decimal value)
    {
        uint quotient;
        float remainderPercent;
        uint maxPower = 7;
        while(!TryUnitDivRem(maxPower, value, out quotient, out remainderPercent))
        {
            maxPower--;
            if (maxPower <= 0)
                break;
        }
        float unitValue = quotient + remainderPercent;
        if (maxPower <= 0)
            unitValue = (float)value;

        FixedString32Bytes unitString = new FixedString32Bytes(unitValue.ToString("##.0"));
        if(maxPower > 0)
        {
            unitString.Append(unitChars[maxPower - 1]);
        }
        return unitString;
    }
    public static bool TryUnitDivRem(uint unitPower, decimal value, out uint quotient, out float remainderPercent)
    {
        quotient = 0;
        remainderPercent = 0;
        decimal unit = 1;
        if (unitPower <= 0)
            return false;
        while(unitPower > 0)
        {
            unit = unit * 1000;
            unitPower--;
        }
        if(value < unit)
        {
            return false;
        }
        while(value > unit)
        {
            value -= unit;
            quotient++;
        }
        remainderPercent = (float)(value / unit);
        return true;
    }
}
