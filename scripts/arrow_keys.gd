extends Node

@export var arrow_up: Sprite2D
@export var arrow_down: Sprite2D
@export var arrow_left: Sprite2D
@export var arrow_right: Sprite2D

func _unhandled_input(event: InputEvent):
	if event.is_action_pressed("arrow-up"):
		scale_button(arrow_up)
	elif event.is_action_pressed("arrow-down"):
		scale_button(arrow_down)
	elif event.is_action_pressed("arrow-left"):
		scale_button(arrow_left)
	elif event.is_action_pressed("arrow-right"):
		scale_button(arrow_right)
	
func scale_button(arrow_key: Sprite2D):
	arrow_key.scale = Vector2.ONE * 3
	var time = 0.0
	while time <= .1:
		time += get_process_delta_time()
		arrow_key.scale = lerp(Vector2.ONE * 2, Vector2.ONE, time * 10)
		await get_tree().create_timer(get_process_delta_time()).timeout
	pass
