extends Resource
class_name Model

#var all_beats: Stack[BeatValue]
var current_beat: BeatValue

func add_beat():
	pass
func hit_note(index):
	
	pass
func pop_beat():
	#current_beat = all_beats.pop()
	pass

class BeatValue:
	var note_indexes: Array[int]
	func _init(indexes: Array[int]):
		note_indexes = indexes
		pass
