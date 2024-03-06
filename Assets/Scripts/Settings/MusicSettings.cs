using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EscapeTower.GameSettings {

    [Serializable]
    public class MusicSettings {
        public List<AudioClip> playlist = new();
    }
}