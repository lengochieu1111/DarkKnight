using System.Collections;
using System.Collections.Generic;
using HIEU_NL.Platformer.Script.Effect;
using HIEU_NL.Platformer.Script.Interface;
using UnityEngine;

public class DamageEffect_Platformer : BaseEffect_Platformer
{
    [SerializeField] private SpriteRenderer _number_1_Sprite;
    [SerializeField] private SpriteRenderer _number_2_Sprite;
    [SerializeField] private Sprite[] _numberSpriteArray;

    public void Setup(HitData hitData)
    {
        if (hitData.Damage / 10 != 0)
        {
            _number_1_Sprite.sprite = GetSpite(hitData.Damage / 10);
        }
        else
        {
            _number_1_Sprite.sprite = null;
        }
        
        _number_2_Sprite.sprite = GetSpite(hitData.Damage % 10);
    }

    private Sprite GetSpite(int number)
    {
        return _numberSpriteArray[number];
    }
    

}
