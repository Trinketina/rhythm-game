extends Node2D

@export var note: PackedScene
var notes = []

func _ready():
	loop_spawn_notes()

func loop_spawn_notes():
	while true:
		var new_note = note.instantiate()
		add_child(new_note)
		#notes.append(new_note)
		await get_tree().create_timer(1).timeout
	pass
