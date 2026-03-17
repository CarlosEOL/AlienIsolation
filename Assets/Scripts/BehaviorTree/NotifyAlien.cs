using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

#if UNITY_EDITOR
[CreateAssetMenu(menuName = "Behavior/Event Channels/Notify Alien")]
#endif
[Serializable, GeneratePropertyBag]
[EventChannelDescription(name: "Notify Alien", message: "Notify [Alien] [Last] [Position]", category: "Action/GameObject", id: "c033ad17e34b19acc8d9b28fc2806cbf")]
public sealed partial class NotifyAlien : EventChannel<GameObject, Transform, Vector3> { }

