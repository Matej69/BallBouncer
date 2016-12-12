using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
public class MovablePlatformQuantityInfo {
    List<PlatformMovableInfo> movablePlatforms;
    /*void Init(int _normal, int _circle, int _triangle, int _curve) {
        normal = _normal;
        circle = _circle;
    }
         
}
*/
public class PlatformMovableInfo  {
    public int shape;
    public int surface;
    public PlatformMovableInfo(int _shape, int _surface) {
        shape = _shape;
        surface = _surface;
    }
}

public class PlatformEnvironmentInfo{
    public double posX, posY;
    public double rotZ;
    public int shape;
    public int surface;
    public PlatformEnvironmentInfo(double _posX, double _posY, double _rotZ, int _shape, int _surface) {
        posX = _posX;
        posY = _posY;
        rotZ = _rotZ;
        shape = _shape;
        surface = _surface;
    }
}
