using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Unity.Netcode;
using UnityEngine.Assertions;

public class Damageable : MonoBehaviour
{
    [Tooltip("Time that this gameObject is invulnerable for, after receiving damage.")]
    public float invulnerabiltyTime;
    public bool isInvulnerable { get; set; }
    public UnityEvent OnReceiveDamage, OnHitWhileInvulnerable, OnBecomeVulnerable;
    protected float m_timeSinceLastHit = 0.0f;
    protected Collider m_Collider;

    [Tooltip("When this gameObject is damaged, these other gameObjects are notified.")]
    [EnforceType(typeof(IMessageReceiver))]
    public List<MonoBehaviour> onDamageMessageReceivers = new List<MonoBehaviour>();


    public void OnValidate()
    {
        for (var i = 0; i < onDamageMessageReceivers.Count; i++)
        {
            var receiver = onDamageMessageReceivers[i] as IMessageReceiver;
            Assert.IsNotNull(receiver, $"{nameof(IMessageReceiver)} at index {i.ToString()}  \"{onDamageMessageReceivers[i].name}\" has no {nameof(IMessageReceiver)} interface.");
        }
    }
    public void ApplyDamage(DamageMessage msg)
    {
        if (isInvulnerable) return;
        for (var i = 0; i < onDamageMessageReceivers.Count; ++i)
        {
            var receiver = onDamageMessageReceivers[i] as IMessageReceiver;
            receiver.OnReceiveMessage(MessageType.DAMAGED, msg);
        }
    }

    [System.Serializable]
    public class DamageMessage
    {

        public ulong damagerClientId;
        public float amount;
        public Vector3 direction;

        public DamageMessage(ulong damagerClientId, float amount, Vector3 direction)
        {
            this.damagerClientId = damagerClientId;
            this.amount = amount;
            this.direction = direction;
        }
    }
}
