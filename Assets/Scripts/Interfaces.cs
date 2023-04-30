using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable // 攻撃を受けられる場合のインターフェース
{
    void Attacked(float damage);
}
