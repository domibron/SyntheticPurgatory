using System;

[AttributeUsage(AttributeTargets.Field)]
public class StatAssignableAttribute : Attribute
{
    public string TargetStatName { get; }

    public StatAssignableAttribute(string targetStatName)
    {
        TargetStatName = targetStatName;
    }
}
