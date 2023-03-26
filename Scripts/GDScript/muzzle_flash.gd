class_name MuzzleFlash
extends Node3D

@export var _particles: GPUParticles3D;
@export var _life_time: float = 0.3;

func _ready():
	_particles.emitting = true;

	await get_tree().create_timer(_life_time).timeout;
	queue_free();
