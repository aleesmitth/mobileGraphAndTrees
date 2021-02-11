using System;

public class PositionOccupiedByNodeException : Exception {
    public PositionOccupiedByNodeException(string message) : base(message) { }
}