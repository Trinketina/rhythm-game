extends Node2D
@export var note_speed: int = 20

func _process(delta: float) -> void:
	position.y += note_speed * delta
	pass
