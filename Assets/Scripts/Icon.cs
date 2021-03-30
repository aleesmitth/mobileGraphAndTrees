using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Icon : MonoBehaviour {
    public SpriteRenderer artwork;

    public void SetArtworkSprite(Sprite art) {
        artwork.sprite = art;
    }
}
