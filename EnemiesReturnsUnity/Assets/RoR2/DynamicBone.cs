// DynamicBone
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Dynamic Bone/Dynamic Bone")]
public class DynamicBone : MonoBehaviour
{
	public enum UpdateMode
	{
		Normal,
		AnimatePhysics,
		UnscaledTime
	}

	public enum FreezeAxis
	{
		None,
		X,
		Y,
		Z
	}

	private class Particle
	{
		public Transform m_Transform;

		public int m_ParentIndex = -1;

		public float m_Damping;

		public float m_Elasticity;

		public float m_Stiffness;

		public float m_Inert;

		public float m_Radius;

		public float m_BoneLength;

		public Vector3 m_Position = Vector3.zero;

		public Vector3 m_PrevPosition = Vector3.zero;

		public Vector3 m_EndOffset = Vector3.zero;

		public Vector3 m_InitLocalPosition = Vector3.zero;

		public Quaternion m_InitLocalRotation = Quaternion.identity;
	}

	public Transform m_Root;

	public float m_UpdateRate = 60f;

	public UpdateMode m_UpdateMode;

	[Range(0f, 1f)]
	public float m_Damping = 0.1f;

	public AnimationCurve m_DampingDistrib;

	[Range(0f, 1f)]
	public float m_Elasticity = 0.1f;

	public AnimationCurve m_ElasticityDistrib;

	[Range(0f, 1f)]
	public float m_Stiffness = 0.1f;

	public AnimationCurve m_StiffnessDistrib;

	[Range(0f, 1f)]
	public float m_Inert;

	public AnimationCurve m_InertDistrib;

	public float m_Radius;

	public AnimationCurve m_RadiusDistrib;

	public float m_EndLength;

	public Vector3 m_EndOffset = Vector3.zero;

	public Vector3 m_Gravity = Vector3.zero;

	public Vector3 m_Force = Vector3.zero;

	public List<DynamicBoneCollider> m_Colliders;

	public List<Transform> m_Exclusions;

	public FreezeAxis m_FreezeAxis;

	public bool m_DistantDisable;

	public Transform m_ReferenceObject;

	public float m_DistanceToObject = 20f;

	[Tooltip("Check this if you want the bone to be dynamic even on low performance HW")]
	public bool neverOptimize;

	private Vector3 m_LocalGravity = Vector3.zero;

	private Vector3 m_ObjectMove = Vector3.zero;

	private Vector3 m_ObjectPrevPosition = Vector3.zero;

	private float m_BoneTotalLength;

	private float m_ObjectScale = 1f;

	private float m_Time;

	private float m_Weight = 1f;

	private bool m_DistantDisabled;

	private List<Particle> m_Particles = new List<Particle>();

	private void Start()
	{
		
	}

	private void FixedUpdate()
	{

	}

	private void Update()
	{

	}

	private void LateUpdate()
	{

	}

	private void PreUpdate()
	{

	}

	private void CheckDistance()
	{

	}

	private void OnEnable()
	{
	}

	private void OnDisable()
	{
	}

	private void OnValidate()
	{

	}

	private void OnDrawGizmosSelected()
	{

	}

	public void SetWeight(float w)
	{

	}

	public float GetWeight()
	{
		return 0;
	}

	private void UpdateDynamicBones(float t)
	{

	}

	private void SetupParticles()
	{

	}

	private void AppendParticles(Transform b, int parentIndex, float boneLength)
	{

	}

	private void InitTransforms()
	{

	}

	private void ResetParticlesPosition()
	{

	}

	private void UpdateParticles1()
	{

	}

	private void UpdateParticles2()
	{

	}

	private void SkipUpdateParticles()
	{

	}

	private static Vector3 MirrorVector(Vector3 v, Vector3 axis)
	{
		return v;
	}

	private void ApplyParticlesToTransforms()
	{

	}
}
