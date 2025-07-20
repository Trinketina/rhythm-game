extends Node2D
class_name Beat
@export var model: Model
@export var note_symbols: Array[Sprite2D]
var current_score = "NA"
var note_scores: Array[String]

func start_beat(note_indexes: Array[int]):
	for note_index in note_indexes:
		note_symbols[note_index].show()
	#note_scores = [5]
	pass

func press_note(note_index: int):
	if current_score != "NA":
		note_scores[note_index] = current_score
	pass

func end_beat():
	for note in note_symbols:
		note.hide()
	pass
