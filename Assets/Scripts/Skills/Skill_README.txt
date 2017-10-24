### Skills
- Creating a skill is mostly generic and relies heavily on the Animator States to properly control timings. 

### Creating a new skill
1. Create a new script that extends Skill. Add to player prefab root. 

2. Implement the interface. 
	- ConditionsMet		: The conditions required for this skill to activate (ex. mana, only grounded, etc.)
	- ConsumeResources	: Should be called when the skill is cast. Take away any resources the skills uses (ex. mana, stamina)
	- Activate			: The action that should occur if and when the skills hits another player. 

3. Add some custom functions for certain events. 
	For example AntiPhase attack:
	- animState.OnStateEntered += SkillStart() { // switch weapon form }
	- AnimEvent_APA_ColliderActivate() { // enable the weapons collider to start detecting if the weapon hits anything }
		- This is an arbitary function, can be named anything. 
	- AnimEvent_APA_ColliderDeactivate() { // disable the weapon collider to stop detecting hits }
		- This is an arbitary function, can be named anything. 
	- animState.OnStateExited += SkillEnd() { // switch weapon form back to initial. Called when anim is done or if interrupted }

3. Create a new animation state in the animator

4. Add the SkillStateMachine to the state. Give it an ID and remember it. 

5. Add an animation clip to the state. 

6. Create animation events in the clip to call any custom functions
	For example AntiPhase attack you would call AnimEvent_APA_ColliderActivate() 

7. Disable the script in PlayerSetup script. 