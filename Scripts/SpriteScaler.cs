using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;


namespace radiants.SpriteScaler
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(RectTransform))]
	public class SpriteScaler : MonoBehaviour
	{
		#region Util

		private static Material GetUnityDefaultSpriteMaterial()
		{
#if UNITY_EDITOR
			return UnityEditor.AssetDatabase.GetBuiltinExtraResource<Material>("Sprites-Default.mat");
#else
			return Resources.GetBuiltinResource<Material>("Sprites-Default.mat");
#endif
		}

		#endregion

		#region Properties / Observables

		private Subject<Unit> OnRectTransformDimensionsChangeSubject = new Subject<Unit>();

		[SerializeField]
		private SpriteReactiveProperty _RenderSprite = new SpriteReactiveProperty();
		public Sprite renderSprite
		{
			get { return _RenderSprite.Value; }
			set { _RenderSprite.Value = value; }
		}

		[SerializeField]
		private BoolReactiveProperty _PreserveAspect = new BoolReactiveProperty(false);
		public bool preserveAspect
		{
			get { return _PreserveAspect.Value; }
			set{ _PreserveAspect.Value = value; }
		}

		[SerializeField]
		private ColorReactiveProperty _Color = new ColorReactiveProperty(Color.white);
		public Color color
		{
			get { return _Color.Value; }
			set { _Color.Value = value; }
		}

		[SerializeField]
		private BoolReactiveProperty _FlipX = new BoolReactiveProperty(false);
		public bool flipX
		{
			get { return _FlipX.Value; }
			set { _FlipX.Value = value; }
		}

		[SerializeField]
		private BoolReactiveProperty _FlipY = new BoolReactiveProperty(false);
		public bool flipY
		{
			get { return _FlipY.Value; }
			set { _FlipY.Value = value; }
		}

		[SerializeField]
		private SpriteDrawModeReactiveProperty _DrawMode = new SpriteDrawModeReactiveProperty(SpriteDrawMode.Simple);
		public SpriteDrawMode drawMode
		{
			get { return _DrawMode.Value; }
			set { _DrawMode.Value = value; }
		}

		//todo scale for tiled/sliced
		[SerializeField]
		private FloatReactiveProperty _ScaleForTiledAndSliced = new FloatReactiveProperty(1f);
		public float ScaleForTiledAndSliced
		{
			get { return _ScaleForTiledAndSliced.Value; }
			set { _ScaleForTiledAndSliced.Value = value; }
		}

		[SerializeField]
		private SpriteMaskInteractionReactiveProperty _MaskInteraction = new SpriteMaskInteractionReactiveProperty(SpriteMaskInteraction.None);
		public SpriteMaskInteraction maskInteraction
		{
			get { return _MaskInteraction.Value; }
			set { _MaskInteraction.Value = value; }
		}

		[SerializeField]
		private SpriteSortPointReactiveProperty _SortPoint = new SpriteSortPointReactiveProperty(SpriteSortPoint.Center);
		public SpriteSortPoint sortPoint
		{
			get { return _SortPoint.Value; }
			set { _SortPoint.Value = value; }
		}

		[SerializeField]
		private MaterialReactiveProperty _Material = new MaterialReactiveProperty();
		public Material customMaterial
		{
			get { return _Material.Value; }
			set { _Material.Value = value; }
		}

		[SerializeField]
		private IntReactiveProperty _SortingLayerID = new IntReactiveProperty(0);
		public int sortingLayerID
		{
			get { return _SortingLayerID.Value; }
			set { _SortingLayerID.Value = value; }
		}

		[SerializeField]
		private IntReactiveProperty _OrderInLayer = new IntReactiveProperty(0);
		public int orderInLayer
		{
			get { return _OrderInLayer.Value; }
			set { _OrderInLayer.Value = value; }
		}

		#endregion

		#region Subscribes

		private CompositeDisposable LifetimeDisposables = new CompositeDisposable();

		private void SubscribeLifetimeObservables()
		{
			_RenderSprite.Subscribe(_spr => TargetRenderer.sprite = _spr)
				.AddTo(LifetimeDisposables);

			_Color.Subscribe(_col => TargetRenderer.color = _col)
				.AddTo(LifetimeDisposables);

			_FlipX.Subscribe(_flip => TargetRenderer.flipX = _flip)
				.AddTo(LifetimeDisposables);
			_FlipY.Subscribe(_flip => TargetRenderer.flipY = _flip)
				.AddTo(LifetimeDisposables);

			_DrawMode.Subscribe(_mode => TargetRenderer.drawMode = _mode)
				.AddTo(LifetimeDisposables);
			_MaskInteraction.Subscribe(_value => TargetRenderer.maskInteraction = _value)
				.AddTo(LifetimeDisposables);
			_SortPoint.Subscribe(_value => TargetRenderer.spriteSortPoint = _value)
				.AddTo(LifetimeDisposables);

			_Material.Subscribe(_value =>
				{
					if (_value == null) _value = GetUnityDefaultSpriteMaterial();
					TargetRenderer.sharedMaterial = _value;
				})
				.AddTo(LifetimeDisposables);

			_SortingLayerID.Subscribe(_value => TargetRenderer.sortingLayerID = _value)
				.AddTo(LifetimeDisposables);
			_OrderInLayer.Subscribe(_value => TargetRenderer.sortingOrder = _value)
				.AddTo(LifetimeDisposables);
		}

		private CompositeDisposable ActiveTimeDisposables = new CompositeDisposable();

		private void SubscribeActiveTimeObservables()
		{
			Observable.CombineLatest(_RenderSprite, _PreserveAspect, _DrawMode, _ScaleForTiledAndSliced,
					(_1, _2, _3, _4) => Unit.Default)
				.Subscribe(_ => AdjustSpriteScale())
				.AddTo(ActiveTimeDisposables);

			OnRectTransformDimensionsChangeSubject
				.Subscribe(_ => AdjustSpriteScale())
				.AddTo(ActiveTimeDisposables);
		}

		public void SyncAllProperties()
		{
			TargetRenderer.sprite = renderSprite;
			TargetRenderer.color = color;
			TargetRenderer.flipX = flipX;
			TargetRenderer.flipY = flipY;
			TargetRenderer.drawMode = drawMode;
			TargetRenderer.maskInteraction = maskInteraction;
			TargetRenderer.spriteSortPoint = sortPoint;

			if (customMaterial == null)
				TargetRenderer.sharedMaterial = GetUnityDefaultSpriteMaterial();
			else
				TargetRenderer.sharedMaterial = customMaterial;

			TargetRenderer.sortingLayerID = sortingLayerID;
			TargetRenderer.sortingOrder = orderInLayer;

			AdjustSpriteScale();
		}

		#endregion

		#region references

		private RectTransform _MyRectTransform;
		private RectTransform MyRectTransform
		{ get { if (_MyRectTransform == null) _MyRectTransform = GetComponent<RectTransform>(); return _MyRectTransform; } }

		[SerializeField]
		private SpriteRenderer _TargetRenderer;
		private SpriteRenderer TargetRenderer
		{ get { if (_TargetRenderer == null) CreateAndLinkChildSprite(); return _TargetRenderer; } }
		[SerializeField]
		private Transform _TargetTransform;
		private Transform TargetTransform
		{ get { if (_TargetTransform == null) CreateAndLinkChildSprite(); return _TargetTransform; } }

		#endregion

		#region Make Child Sprite

		private void CreateAndLinkChildSprite()
		{
			GameObject child = new GameObject("_");
			//check
			//child.hideFlags = HideFlags.DontSave;
			child.hideFlags = HideFlags.HideAndDontSave;
			_TargetTransform = child.transform;
			child.transform.SetParent(MyRectTransform);
			child.transform.SetSiblingIndex(0);
			child.transform.localRotation = Quaternion.identity;
			child.layer = gameObject.layer;
			_TargetRenderer = child.AddComponent<SpriteRenderer>();
		}

		#endregion


		#region Monobehaviour callback

		private void Awake()
		{
			SubscribeLifetimeObservables();

#if UNITY_EDITOR
			UnityEditor.Undo.undoRedoPerformed += SyncAllProperties;
#endif
		}

		private void OnRectTransformDimensionsChange()
		{
			OnRectTransformDimensionsChangeSubject.OnNext(Unit.Default);
		}

		private void OnEnable()
		{
			TargetRenderer.enabled = true;
			SubscribeActiveTimeObservables();
		}

		private void OnDisable()
		{
			TargetRenderer.enabled = false;
			ActiveTimeDisposables.Clear();
		}

		private void OnDestroy()
		{
			LifetimeDisposables.Dispose();
			ActiveTimeDisposables.Dispose();

			//Destroy Hidden Child
			if (Application.isPlaying)
				Destroy(TargetRenderer.gameObject);
			else
				DestroyImmediate(TargetRenderer.gameObject, false);
#if UNITY_EDITOR
			UnityEditor.Undo.undoRedoPerformed -= SyncAllProperties;
#endif
		}

		#endregion

		#region Modify Scales

		public void AdjustSpriteScale()
		{
			if (renderSprite == null) return;

			var originalBounds = renderSprite.bounds.size;
			Vector3 scale;

			if (drawMode == SpriteDrawMode.Sliced
				|| drawMode == SpriteDrawMode.Tiled)
			{
				//9-sliced, tiled
				float refScale = renderSprite.pixelsPerUnit * ScaleForTiledAndSliced;
				scale = new Vector3(refScale, refScale, 1f);
				TargetRenderer.size = MyRectTransform.rect.size / refScale;
			}
			else
			{
				//normal
				scale = new Vector3(MyRectTransform.rect.size.x / originalBounds.x, MyRectTransform.rect.size.y / originalBounds.y, 1f);
				if (preserveAspect)
					PreserveAspectScale(ref scale);
			}

			TargetTransform.localScale = scale;
			TargetTransform.localPosition = MyRectTransform.rect.center;
		}

		private void PreserveAspectScale(ref Vector3 _scale)
		{
			if (_scale.x > _scale.y)
			{
				_scale.x = _scale.y;
			}
			else
			{
				_scale.y = _scale.x;
			}
		}

		#endregion
	}

}