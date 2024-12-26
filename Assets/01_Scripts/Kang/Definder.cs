using UnityEngine;

public static class Definder
{
    private static Camera _mainCam;
    public static Camera MainCam { 
        get{
            if(!_mainCam)
                _mainCam = Camera.main;

            return _mainCam;
        }
        private set {
            _mainCam = value;
        }
    }
    private static Player _player;
    public static Player Player
    {
        get
        {
            if (!_player)
                _player = Object.FindAnyObjectByType<Player>();
            return _player;
        }
        private set
        {
            _player = value;
        }
    }
}
