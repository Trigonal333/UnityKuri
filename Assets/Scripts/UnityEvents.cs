using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;

public class DestinationEvent : UnityEvent<Vector3, int> {}
public class MultiplicationEvent : UnityEvent<Vector3> {}
public class SpawnEvent : UnityEvent<Vector3, Quaternion> {}
public class DestroyEvent : UnityEvent<int> {}
public class AnimationEvent : UnityEvent<float, float, bool> {}
public class DestroyOrRemoveEvent : UnityEvent<int, bool> {}