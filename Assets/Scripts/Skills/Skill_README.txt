### Skills
- Creating a skill is mostly generic and relies heavily on the Animator States to properly control timings. 

### Creating a new skill
1. Create a new script that extends Skill. Add to player prefab root. 

2. Implement the interface. 
	- ConditionsMet		: The conditions required for this skill to activate (ex. mana, only grounded, etc.)
	- ConsumeResources	: Should be called when the skill is cast. Take away any resources the skills uses (ex. mana, stamina)
	- Activate			: The action that should occur if and when the skills hits another player. 

3. Override functions (remember to always calls base).
	- Awake				: If you require more references.
	- SkillStart		: For any setup the instant the skill starts casting (ex. start listening for hits, change appearance, etc.)
	- SkillEnd			: For any actions required when the skills ends or is interrupted. (ex. stop listening for hits)

4. Add any custom functions / logic.

5. Create a new animation state in the animator.

6. Add the SkillStateMachine to the state. Give it an ID and set Anim State ID field on the script (in the editor) to the same value. 

7. Add an animation clip to the state. 

8. Create animation events in the clip to call any custom functions.
	- For example BasicAttack calls AnimEvent_ActivateCollider halfway through to start hit detection. 

9. DO NOT disable the script in PlayerSetup. Ensure your functions do the correct network checking. 