# Known Issues:

- When picking up items such as when mining blocks while building, and said blocks are in offhand, they get stacked into inventory instead of offhand, eventually forcing you to manually replace the stack into offhand. To avoid this, try to keep blocks in hotbar instead.

- Pressing 'Use Offhand Item' keybind and releasing it too quickly, then holding the keybind again, results in the item being fired once and input being blocked. You can do short bursts, but not "a few frames" short.

- Going click by click with the 'Use Offhand Item' keybind instead of holding it may cause some issues.

- Pressing 'Use Offhand Item' keybind for less than 1 tick will cause nothing to happen. This cannot be circumvented at the moment as if the user right clicks a chest to open it with autopause enabled and the 1 tick delay isn't there, the game will pause the frame after an item use animation happens, making you unable to move items around.

- Slot is partially on top of / covered by crafting menu if it's scrolled down. 
# Planned Features:

- Changing offhand slot texture

- Adding a toggle that allows dragging the offhand slot

- Adding a clickable toggle to the offhand slot, which decides whether holding the 'Use Offhand Slot' key will use the item's normal attack(LMB), or it's alternate attack(RMB)
- Making offhand slot visible even when inventory is closed

- Allowing use of 2 items simultaneously, like shooting a bow and swinging a sword together. May require rewriting half of the mod and adding a lot of new code.

- Adding a check to disable 'Use Offhand Item' keybind if it is set to mouse2(RMB) and the player is currently holding a dual-function(LMB/RMB) item in their selected hotbar slot.
- Adding a modifier key that holds offhand item *while held* but still requires manual LMB/RMB presses?
