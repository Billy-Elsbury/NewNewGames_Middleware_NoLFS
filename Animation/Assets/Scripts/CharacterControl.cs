using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace Supercyan.AnimalPeopleSample
{
    public class CharacterControl : NetworkBehaviour
    {
        private const float pickUpTolerance = 0.4f;
        public Transform rightHandGrip;
        

        [SerializeField] private float m_moveSpeed = 2;
        [SerializeField] private float m_turnSpeed = 200;
        [SerializeField] private float m_jumpForce = 4;

        [SerializeField] private Animator m_animator = null;
        [SerializeField] private Rigidbody m_rigidBody = null;

        private float m_currentV = 0;
        private float m_currentH = 0;

        private readonly float m_interpolation = 10;
        private readonly float m_walkScale = 0.33f;
        private readonly float m_backwardsWalkScale = 0.16f;
        private readonly float m_backwardRunScale = 0.66f;

        private bool m_wasGrounded;
        private Vector3 m_currentDirection = Vector3.zero;

        private float m_jumpTimeStamp = 0;
        private float m_minJumpInterval = 0.25f;
        private bool m_jumpInput = false;

        private bool m_isGrounded;

        private List<Collider> m_collisions = new List<Collider>();

        Transform rightHandGripTr;

        ObjectScript focusObject;
        ObjectSpawner objectSpawner;
        CloneManagerScript theClones;
        private void Awake()
        {
            if (!m_animator) { gameObject.GetComponent<Animator>(); }
            if (!m_rigidBody) { gameObject.GetComponent<Animator>(); }

            if (rightHandGrip == null)
            {
                rightHandGrip = m_animator.GetBoneTransform(HumanBodyBones.RightHand);
            }

            theClones = FindObjectOfType<CloneManagerScript>();

            rightHandGripTr = Instantiate(theClones.HandGripCloneTemplate).transform;
        }

        [SerializeField] private Camera playerCamera; // Assign the camera from your prefab

        private void Start()
        {
            // Ensure this setup only runs for the local player
            if (IsLocalPlayer)
            {
                playerCamera.enabled = true;  // Enable the local player's camera
            }
            else
            {
                playerCamera.enabled = false; // Disable remote players' cameras
            }

            objectSpawner = FindObjectOfType<ObjectSpawner>();


        }

        private ObjectScript ClosestObject()
        {
            ObjectScript closestObject = null;
            float closestDistance = float.MaxValue;

            Collider[] allColliders = Physics.OverlapSphere(gameObject.transform.position, 5);
            

            foreach (Collider col in allColliders)
            {
                ObjectScript snowball = col.GetComponent<ObjectScript>();
                if (snowball != null)
                {
                    float distance = Vector3.Distance(transform.position, col.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestObject = snowball;
                    }

                }
            }
            return closestObject;
        }

            private void OnCollisionEnter(Collision collision)
        {
            ContactPoint[] contactPoints = collision.contacts;
            for (int i = 0; i < contactPoints.Length; i++)
            {
                if (Vector3.Dot(contactPoints[i].normal, Vector3.up) > 0.5f)
                {
                    if (!m_collisions.Contains(collision.collider))
                    {
                        m_collisions.Add(collision.collider);
                    }
                    m_isGrounded = true;
                }
            }
        }

        private void OnCollisionStay(Collision collision)
        {
            ContactPoint[] contactPoints = collision.contacts;
            bool validSurfaceNormal = false;
            for (int i = 0; i < contactPoints.Length; i++)
            {
                if (Vector3.Dot(contactPoints[i].normal, Vector3.up) > 0.5f)
                {
                    validSurfaceNormal = true; break;
                }
            }

            if (validSurfaceNormal)
            {
                m_isGrounded = true;
                if (!m_collisions.Contains(collision.collider))
                {
                    m_collisions.Add(collision.collider);
                }
            }
            else
            {
                if (m_collisions.Contains(collision.collider))
                {
                    m_collisions.Remove(collision.collider);
                }
                if (m_collisions.Count == 0) { m_isGrounded = false; }
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            if (m_collisions.Contains(collision.collider))
            {
                m_collisions.Remove(collision.collider);
            }
            if (m_collisions.Count == 0) { m_isGrounded = false; }
        }

        private bool isPickingUp = false;
        private bool isHoldingObject = false;
        private Vector3 ikTargetPosition;
        private Quaternion ikTargetRotation;

        private float maxReachDistance = 1.5f; 
        private float reachSpeed = 3f; 

        private void Update()
        {
            if (!IsOwner) return;
            CharacterMovement();
            CharacterActions();

            rightHandGripTr.position = rightHandGrip.position;
            rightHandGripTr.rotation = rightHandGrip.rotation;
        }

        private void CharacterActions()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                RequestSnowBallSpawnServerRpc();
            }
            
        }

        [ServerRpc]
        private void RequestSnowBallSpawnServerRpc()
        {
            objectSpawner.SpawnSnowBall();
        }


        private void CharacterMovement()
        {
            if (Input.GetKeyDown(KeyCode.F) && !isHoldingObject)
            {
                focusObject = ClosestObject();
            }

            if (Input.GetKeyDown(KeyCode.E) && focusObject != null && !isHoldingObject)
            {
                isPickingUp = true;
                m_animator.SetTrigger("PickUp");
                ikTargetPosition = m_animator.GetBoneTransform(HumanBodyBones.RightHand).position;
            }

            if (!m_jumpInput && Input.GetKey(KeyCode.Space))
            {
                m_jumpInput = true;
            }

            if (Input.GetKeyDown(KeyCode.T) && isHoldingObject)
            {
                ThrowObject();
            }
        }

        private void OnAnimatorIK(int layerIndex)
        {
            if (isPickingUp && focusObject != null)
            {
                Transform handTransform = rightHandGrip;

                Vector3 directionToObject = (focusObject.transform.position - transform.position).normalized;
                float distanceToObject = Vector3.Distance(transform.position, focusObject.transform.position);

                float clampedDistance = Mathf.Min(distanceToObject, maxReachDistance);

                Vector3 targetPosition = transform.position + directionToObject * clampedDistance;

                // Lerp hand to target
                ikTargetPosition = Vector3.Lerp(ikTargetPosition, targetPosition, Time.deltaTime * reachSpeed);
                ikTargetRotation = Quaternion.LookRotation(focusObject.transform.position - handTransform.position);

                m_animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
                m_animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
                m_animator.SetIKPosition(AvatarIKGoal.RightHand, ikTargetPosition);
                m_animator.SetIKRotation(AvatarIKGoal.RightHand, ikTargetRotation);
                
                float distanceToHand = Vector3.Distance(handTransform.position, focusObject.transform.position) - focusObject.transform.localScale.x;
                if (distanceToHand <= pickUpTolerance)
                {
                    AttachObjectToHandTr(focusObject);
                    isPickingUp = false;
                }
            }
            else
            {
                // Reset IK weights
                m_animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
                m_animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
            }
        }

        private void AttachObjectToHandTr(ObjectScript obj)
        {
            var networkObject = obj.GetComponent<NetworkObject>();
            if (networkObject != null)
            {
                //if (IsServer)
                {
                    // Reparent the object to the hand on the server
                    print("Host tried to attach object");
                    networkObject.TrySetParent(rightHandGripTr, false);
                    UpdatePositionAndRotation(obj);
                }
                //else
                {
                    // Request the server to handle reparenting
                    //SubmitReparentRequestServerRpc(networkObject.NetworkObjectId);
                    //UpdatePositionAndRotation(obj);
                }
            }

            var objRigidbody = obj.GetComponent<Rigidbody>();
            if (objRigidbody != null)
            {
                objRigidbody.isKinematic = true;
            }

            var objCollider = obj.GetComponent<Collider>();
            if (objCollider != null)
            {
                objCollider.enabled = false;
            }

            isHoldingObject = true;
        }

        // Sync position and rotation
        private void UpdatePositionAndRotation(ObjectScript obj)
        {
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;
        }


        [ServerRpc]
        private void SubmitReparentRequestServerRpc(ulong objectId)
        {
            NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(objectId, out NetworkObject networkObject);
            if (networkObject != null)
            {
                networkObject.TrySetParent(rightHandGripTr);
            }
        }


        private void ThrowObject()
        {
            if (isHoldingObject && focusObject != null)
            {
                var networkObject = focusObject.GetComponent<NetworkObject>();
                if (networkObject != null)
                {
                    if (IsServer)
                    {
                        // Unparent the object and apply physics changes on the server
                        networkObject.TrySetParent((GameObject)null);
                        ApplyThrowForce(focusObject);
                    }
                    else
                    {
                        // Request the server to handle unparenting and physics
                        SubmitThrowRequestServerRpc(networkObject.NetworkObjectId);
                    }
                }

                isHoldingObject = false;
                focusObject = null;

                // Trigger the throw animation
                m_animator.SetTrigger("Throw");
            }
        }

        [ServerRpc]
        private void SubmitThrowRequestServerRpc(ulong objectId)
        {
            if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(objectId, out NetworkObject networkObject))
            {
                var obj = networkObject.GetComponent<ObjectScript>();
                if (obj != null)
                {
                    networkObject.TrySetParent((GameObject)null);
                    ApplyThrowForce(obj);
                }
            }
        }

        private void ApplyThrowForce(ObjectScript obj)
        {
            var objRigidbody = obj.GetComponent<Rigidbody>();
            if (objRigidbody != null)
            {
                objRigidbody.isKinematic = false;
                objRigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;

                // Apply throw force
                Vector3 throwDirection = transform.forward + Vector3.up * 0.6f;
                float throwForce = 10f;
                objRigidbody.AddForce(throwDirection * throwForce, ForceMode.Impulse);
            }

            var objCollider = obj.GetComponent<Collider>();
            if (objCollider != null)
            {
                objCollider.enabled = true;
            }
        }



        private void FixedUpdate()
        {
            m_animator.SetBool("Grounded", m_isGrounded);
            
            if(Input.GetKeyDown(KeyCode.J))
            m_animator.SetTrigger("Wave");

            TankUpdate();

            m_wasGrounded = m_isGrounded;
            m_jumpInput = false;
        }

        private void TankUpdate()
        {
            float v = Input.GetAxis("Vertical");
            float h = Input.GetAxis("Horizontal");

            bool walk = Input.GetKey(KeyCode.LeftShift);

            if (v < 0)
            {
                if (walk) { v *= m_backwardsWalkScale; }
                else { v *= m_backwardRunScale; }
            }
            else if (walk)
            {
                v *= m_walkScale;
            }

            m_currentV = Mathf.Lerp(m_currentV, v, Time.deltaTime * m_interpolation);
            m_currentH = Mathf.Lerp(m_currentH, h, Time.deltaTime * m_interpolation);

            transform.position += transform.forward * m_currentV * m_moveSpeed * Time.deltaTime;
            transform.Rotate(0, m_currentH * m_turnSpeed * Time.deltaTime, 0);

            m_animator.SetFloat("MoveSpeed", m_currentV);

            JumpingAndLanding();
        }

        private void JumpingAndLanding()
        {
            bool jumpCooldownOver = (Time.time - m_jumpTimeStamp) >= m_minJumpInterval;

            if (jumpCooldownOver && m_isGrounded && m_jumpInput)
            {
                m_jumpTimeStamp = Time.time;
                m_rigidBody.AddForce(Vector3.up * m_jumpForce, ForceMode.Impulse);
            }
        }

        private void LateUpdate()
        {
           
        if (focusObject)
            TurnHeadTo(focusObject.transform);

        }

        private void TurnHeadTo(Transform targetTransform)
        {
            Transform headTransform = m_animator.GetBoneTransform(HumanBodyBones.Head);
            
            headTransform.LookAt(targetTransform);
        }
    }
}