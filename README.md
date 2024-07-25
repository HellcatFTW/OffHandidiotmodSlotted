# Known Issues:

- When picking up items such as when mining blocks while building, and said blocks are in offhand, they get stacked into inventory instead of offhand, eventually forcing you to manually replace the stack into offhand. To avoid this, try to keep blocks in hotbar instead.

- Pressing 'Use Offhand Item' keybind and releasing it too quickly, then holding the keybind again, results in the item being fired once and item use being stopped. You can do short bursts, but not "a few frames" short.

- Going click by click with the 'Use Offhand Item' keybind instead of holding it may cause some very slight visual/animation issues. Needs further testing.

- Pressing 'Use Offhand Item' keybind for less than 1 tick will cause nothing to happen. This is currently necessary as if the user right clicks a chest to open it with autopause enabled and the 1 tick delay isn't there, the game will pause the frame after an item use animation happens, making you unable to move items around. Another reason for this is that it will prevent items from firing if you intended to just open a chest, without autopause, and this is the generally desired behaviour.

- the aforementioned 1-tick delay is host-side, not local, meaning that during multiplayer, by the time a remote player recieves that their delay is over, they'd already have opened the chest/interactable they clicked and their weapon will still fire. Not a big issue, just unpolished.

- Slot is partially on top of / covered by crafting menu if it's scrolled down. 

# Planned Features:

- Changing offhand slot texture

- Adding a toggle that allows dragging the offhand slot

- Adding a clickable toggle to the offhand slot, which decides whether holding the 'Use Offhand Slot' key will use the item's normal attack(LMB), or it's alternate attack(RMB)
- Making offhand slot visible even when inventory is closed

- Allowing use of 2 items simultaneously, like shooting a bow and swinging a sword together. May require rewriting half of the mod and adding a lot of new code.

- Adding a check to disable 'Use Offhand Item' keybind if it is set to mouse2(RMB) and the player is currently holding a dual-function(LMB/RMB) item in their selected hotbar slot.
- Adding a modifier key that holds offhand item *while held* but still requires manual LMB/RMB presses?

- Adding a check where if user has 'Use Offhand Item' keybind set to mouse2(RMB), the mod will see if you are hovering over an interactable(like a chest) and allow the game to take precedence for right clicks, so that the interaction goes through without setting off your 'Use Offhand Item' keybind. this will fix issue 4 and 5.