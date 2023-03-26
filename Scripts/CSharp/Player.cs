using Godot;

public partial class Player : Node3D {
	private Node3D _headNode;
	private RayCast3D _rayCast;
	private Marker3D _muzzleFlashMarker;

	[Export] public NodePath HeadPath { get;set; }
	[Export] NodePath RayCastPath { get;set; }
	[Export] public NodePath MuzzleFlashMarkerPath { get;set; }
	[Export] public float MouseSensitivity { get;set; } = 0.2f;
	[Export] public PackedScene BulletTrailPrefab { get;set; }
	[Export] public PackedScene MuzzleFlashPrefab { get;set; }

	public override void _Ready() {
		this._headNode = this.GetNode<Node3D>(this.HeadPath);
		this._rayCast = this.GetNode<RayCast3D>(this.RayCastPath);
		this._muzzleFlashMarker = this.GetNode<Marker3D>(this.MuzzleFlashMarkerPath);

		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

    public override void _Input(InputEvent e) {
		if (e is InputEventMouseMotion) {
			var mouseMotion = (InputEventMouseMotion) e;
			this.RotateY(Mathf.DegToRad(-mouseMotion.Relative.X * this.MouseSensitivity));

			this._headNode.RotateX(
				Mathf.DegToRad(-mouseMotion.Relative.Y * this.MouseSensitivity)
			);
			var resultRotation = this._headNode.Rotation;
			resultRotation.X = Mathf.Clamp(
				resultRotation.X, Mathf.DegToRad(-85), Mathf.DegToRad(85)
			);
			this._headNode.Rotation = resultRotation;
		}

		if (e.IsPressed() && !e.IsEcho() && Input.IsKeyPressed(Key.Escape)) {
			Input.MouseMode = Input.MouseMode == Input.MouseModeEnum.Captured ?
				Input.MouseModeEnum.Visible :
				Input.MouseModeEnum.Captured;
		}

		if (Input.IsActionJustPressed("fire")) {
			this.OnFire();
		}
	}

	private void OnFire() {
		var bulletTrail = this.BulletTrailPrefab.Instantiate<BulletTrail>();
		var lookAtPoint = this._headNode.GlobalPosition + (
			-this._headNode.GlobalTransform.Basis.Z * 100
		);

		if (this._rayCast.IsColliding()) {
            bulletTrail.MaxDistance = this._muzzleFlashMarker.GlobalPosition.DistanceTo(
				this._rayCast.GetCollisionPoint()
			);
			lookAtPoint = this._rayCast.GetCollisionPoint();
		}

		this._muzzleFlashMarker.AddChild(bulletTrail);
		bulletTrail.LookAt(lookAtPoint, Vector3.Up);

		var muzzleFlash = this.MuzzleFlashPrefab.Instantiate<MuzzleFlash>();
		this._muzzleFlashMarker.AddChild(muzzleFlash);
	}
}
