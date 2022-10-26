using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Unity.Netcode;

public class Damageable : MonoBehaviour
{
    [Tooltip("Time that this gameObject is invulnerable for, after receiving damage.")]
    public float invulnerabiltyTime;
    public bool isInvulnerable { get; set; }
    public UnityEvent OnDeath, OnReceiveDamage, OnHitWhileInvulnerable, OnBecomeVulnerable, OnResetDamage;
    protected float m_timeSinceLastHit = 0.0f;
    protected Collider m_Collider;


    [Tooltip("When this gameObject is damaged, these other gameObjects are notified.")]
    [EnforceType(typeof(IMessageReceiver))]
    public List<MonoBehaviour> onDamageMessageReceivers;

    public void ApplyDamage(DamageMessage data)
    {
    }
    public struct DamageMessage
    {
        public ulong damagerClientId;
        public int amount;
        public Vector3 direction;
    }


    public enum MessageType
    {
        DAMAGED,
        DEAD,
        RESPAWN,
        //Add your user defined message type after
    }

    public interface IMessageReceiver
    {
        void OnReceiveMessage(MessageType type, object sender, object msg);
    }
}
