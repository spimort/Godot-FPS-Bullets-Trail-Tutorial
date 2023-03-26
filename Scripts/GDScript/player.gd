class_name Player
extends Node3D

@export var _head_node: Node3D;
@export var _ray_cast: RayCast3D;
@export var _muzzle_flash_marker: Marker3D;
@export var _bullet_trail_prefab: PackedScene;
@export var _muzzle_flash_prefab: PackedScene;
@export var _mouse_sensitivity: float = 0.2;

func _ready():
	Input.mouse_mode = Input.MOUSE_MODE_CAPTURED;

func _input(event):
	if event is InputEventMouseMotion:
		rotate_y(deg_to_rad(-event.relative.x * _mouse_sensitivity));

		_head_node.rotate_x(deg_to_rad(-event.relative.y * _mouse_sensitivity));
		_head_node.rotation.x = clamp(_head_node.rotation.x, deg_to_rad(-85), deg_to_rad(85));

	if event.is_pressed() and not event.is_echo() and Input.is_key_pressed(KEY_ESCAPE):
		Input.mouse_mode = Input.MOUSE_MODE_VISIBLE \
			if Input.mouse_mode == Input.MOUSE_MODE_CAPTURED else Input.MOUSE_MODE_CAPTURED;

	if Input.is_action_just_pressed("fire"):
		_on_fire();

func _on_fire():
	var bullet_trail: BulletTrail = _bullet_trail_prefab.instantiate();
	var look_at_point: Vector3 = _head_node.global_position + \
		(-_head_node.global_transform.basis.z * 100);

	if _ray_cast.is_colliding():
		bullet_trail.max_distance = _muzzle_flash_marker.global_position.distance_to( \
			_ray_cast.get_collision_point());
		look_at_point = _ray_cast.get_collision_point();

	_muzzle_flash_marker.add_child(bullet_trail);
	bullet_trail.look_at(look_at_point, Vector3.UP);

	var muzzle_flash: MuzzleFlash = _muzzle_flash_prefab.instantiate();
	_muzzle_flash_marker.add_child(muzzle_flash);
