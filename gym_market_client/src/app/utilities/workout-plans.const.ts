export function generateWorkoutPlans(category: string): any[] {
	const cat = (category || '').toLowerCase();
	const isYoga = cat.includes('yoga') || cat.includes('stretch') || cat.includes('pilates');
	const plans = [];

	for (let day = 1; day <= 4; day++) {
		const exercises = [];
		if (isYoga) {
			if (day === 1) {
				exercises.push(
					{
						id: `ex-${day}-1`,
						name: 'Downward Facing Dog',
						duration: '10 mins',
						calories: '45Kcal',
						status: 'done',
						difficulty: '2',
						totalTime: '45sec',
						realTime: '45sec',
						type: 'Yoga',
						description: 'A foundational yoga posture that stretches the shoulders, hamstrings, calves, arches, and hands, while strengthening the arms and legs.',
						steps: [
							'Start on your hands and knees, with hands shoulder-width apart and knees hip-width apart.',
							'Press your hands firmly into the mat and lift your knees off the floor.',
							'Extend your tailbone up toward the ceiling, straightening your legs as much as you can.',
							'Hold and breathe deeply.'
						],
						image: 'https://images.unsplash.com/photo-1544367567-0f2fcb009e0b?w=800&auto=format&fit=crop&q=60'
					},
					{
						id: `ex-${day}-2`,
						name: 'Cobra Stretch',
						duration: '12 mins',
						calories: '50Kcal',
						status: 'done',
						difficulty: '1',
						totalTime: '30sec',
						realTime: '30sec',
						type: 'Stretching',
						description: 'A gentle backbend that stretches the chest, shoulders, and abdomen, while strengthening the spine.',
						steps: [
							'Lie face down on the floor, legs extended.',
							'Place your hands under your shoulders and hug your elbows close to your chest.',
							'Inhale and lift your chest off the floor, keeping your lower ribs down.',
							'Relax your shoulders away from your ears.'
						],
						image: 'https://images.unsplash.com/photo-1518611012118-696072aa579a?w=800&auto=format&fit=crop&q=60'
					},
					{
						id: `ex-${day}-3`,
						name: 'Child\'s Pose',
						duration: '8 mins',
						calories: '20Kcal',
						status: 'active',
						difficulty: '1',
						totalTime: '60sec',
						realTime: '0sec',
						type: 'Resting Pose',
						description: 'A relaxing resting pose that gently stretches the lower back, hips, thighs, and ankles.',
						steps: [
							'Kneel on the floor, sit back on your heels, big toes touching.',
							'Fold your torso forward between your thighs, extending arms forward.',
							'Rest your forehead on the ground and breathe deeply.'
						],
						image: 'https://images.unsplash.com/photo-1599447421416-3414500d18a5?w=800&auto=format&fit=crop&q=60'
					},
					{
						id: `ex-${day}-4`,
						name: 'Warrior II',
						duration: '10 mins',
						calories: '40Kcal',
						status: 'todo',
						difficulty: '3',
						totalTime: '45sec',
						realTime: '0sec',
						type: 'Yoga Strength',
						description: 'A powerful standing pose that strengthens the legs and core while opening the chest and shoulders.',
						steps: [
							'Stand with feet 3-4 feet apart.',
							'Turn right foot out 90 degrees, bend right knee over ankle.',
							'Raise arms parallel to floor, gaze over right hand.',
							'Hold and breathe.'
						],
						image: 'https://images.unsplash.com/photo-1506126613408-eca07ce68773?w=800&auto=format&fit=crop&q=60'
					}
				);
			} else if (day === 2) {
				exercises.push(
					{
						id: `ex-${day}-1`,
						name: 'Cat-Cow Stretch',
						duration: '8 mins',
						calories: '25Kcal',
						status: 'todo',
						difficulty: '1',
						totalTime: '45sec',
						realTime: '0sec',
						type: 'Flexibility',
						description: 'A simple flow to warm up the spine, improve posture, and stretch the back and neck.',
						steps: [
							'Start on your hands and knees in a tabletop position.',
							'Inhale, drop your belly towards the mat, lift your chin and chest (Cow).',
							'Exhale, draw your belly to your spine, round your back toward the ceiling (Cat).',
							'Continue flowing with your breath.'
						],
						image: 'https://images.unsplash.com/photo-1506126613408-eca07ce68773?w=800&auto=format&fit=crop&q=60'
					},
					{
						id: `ex-${day}-2`,
						name: 'Bridge Pose',
						duration: '10 mins',
						calories: '40Kcal',
						status: 'todo',
						difficulty: '2',
						totalTime: '30sec',
						realTime: '0sec',
						type: 'Pilates',
						description: 'Stretches the chest, neck, and spine, while strengthening the back, glutes, and hamstrings.',
						steps: [
							'Lie on your back, knees bent, feet flat on the floor, hip-width apart.',
							'Press feet and arms into the floor, lift hips toward the ceiling.',
							'Clasp hands under your back and lift chest.',
							'Hold for several breaths, then release.'
						],
						image: 'https://images.unsplash.com/photo-1544367567-0f2fcb009e0b?w=800&auto=format&fit=crop&q=60'
					},
					{
						id: `ex-${day}-3`,
						name: 'Sphinx Pose',
						duration: '10 mins',
						calories: '30Kcal',
						status: 'todo',
						difficulty: '1',
						totalTime: '60sec',
						realTime: '0sec',
						type: 'Yoga',
						description: 'A gentle backbend that helps strengthen the spine, stretch the chest and lungs.',
						steps: [
							'Lie on your belly, forearms on the floor, elbows under your shoulders.',
							'Press tops of feet and thighs firmly into the ground.',
							'Lift your chest and head up, drawing your shoulders down.',
							'Keep your gaze forward.'
						],
						image: 'https://images.unsplash.com/photo-1599447421416-3414500d18a5?w=800&auto=format&fit=crop&q=60'
					}
				);
			} else {
				exercises.push(
					{
						id: `ex-${day}-1`,
						name: 'Tree Pose',
						duration: '8 mins',
						calories: '20Kcal',
						status: 'todo',
						difficulty: '2',
						totalTime: '30sec',
						realTime: '0sec',
						type: 'Balance',
						description: 'Improves balance, focus, and strengthens the legs and ankles.',
						steps: [
							'Stand tall, shift weight to your left foot.',
							'Place sole of right foot on inner left thigh or calf (avoid knee).',
							'Bring hands to prayer position in front of chest.',
							'Find a focus point and breathe.'
						],
						image: 'https://images.unsplash.com/photo-1506126613408-eca07ce68773?w=800&auto=format&fit=crop&q=60'
					},
					{
						id: `ex-${day}-2`,
						name: 'Seated Forward Fold',
						duration: '10 mins',
						calories: '35Kcal',
						status: 'todo',
						difficulty: '1',
						totalTime: '45sec',
						realTime: '0sec',
						type: 'Flexibility',
						description: 'Deep stretch for the hamstrings and spine, helping to calm the nervous system.',
						steps: [
							'Sit with legs extended straight in front of you.',
							'Inhale and reach arms overhead.',
							'Exhale, hinge at your hips and fold forward, reaching for feet.',
							'Hold and lengthen your spine.'
						],
						image: 'https://images.unsplash.com/photo-1518611012118-696072aa579a?w=800&auto=format&fit=crop&q=60'
					}
				);
			}
		} else {
			if (day === 1) {
				exercises.push(
					{
						id: `ex-${day}-1`,
						name: 'Abdominal Crunches',
						duration: '10 mins',
						calories: '40Kcal',
						status: 'done',
						difficulty: '2',
						totalTime: '45sec',
						realTime: '45sec',
						type: 'Abdominal muscles',
						description: 'An excellent exercise to isolate and strengthen your abdominal core muscles, improving core stability and posture.',
						steps: [
							'Lie on your back with knees bent and feet flat on the floor.',
							'Place your hands lightly behind your head or crossed over your chest.',
							'Contract your abdominals, exhale, and lift your upper body, keeping your lower back on the floor.',
							'Inhale as you slowly lower back to the starting position.'
						],
						image: 'https://images.unsplash.com/photo-1571019613454-1cb2f99b2d8b?w=800&auto=format&fit=crop&q=60'
					},
					{
						id: `ex-${day}-2`,
						name: 'Jumping on Ball',
						duration: '15 mins',
						calories: '90Kcal',
						status: 'done',
						difficulty: '3',
						totalTime: '60sec',
						realTime: '60sec',
						type: 'Cardio',
						description: 'Dynamic cardio workout on a stability ball, designed to raise heart rate, burn fat, and engage stabilizing muscles.',
						steps: [
							'Sit upright on a stability ball with feet flat on the floor, core engaged.',
							'Bounce gently on the ball by driving through your heels.',
							'Incorporate arm swings or punches to increase intensity.',
							'Keep your back straight and shoulders relaxed throughout.'
						],
						image: 'https://images.unsplash.com/photo-1476480862126-209bfaa8edc8?w=800&auto=format&fit=crop&q=60'
					},
					{
						id: `ex-${day}-3`,
						name: 'Dumbbell Rows',
						duration: '10 mins',
						calories: '50Kcal',
						status: 'active',
						difficulty: '2',
						totalTime: '30sec',
						realTime: '0sec',
						type: 'With dumbbells',
						description: 'Strength exercise focusing on the upper back and lats, helping build a strong and balanced upper body.',
						steps: [
							'Place your left knee and left hand flat on a bench, keeping your back straight.',
							'Hold a dumbbell in your right hand, arm extended toward the floor.',
							'Pull the dumbbell up to your rib cage, squeezing your shoulder blade.',
							'Lower the weight under control to the start position.'
						],
						image: 'https://images.unsplash.com/photo-1583454110551-21f2fa2afe61?w=800&auto=format&fit=crop&q=60'
					},
					{
						id: `ex-${day}-4`,
						name: 'Jumping Jacks',
						duration: '10 mins',
						calories: '110Kcal',
						status: 'todo',
						difficulty: '2',
						totalTime: '60sec',
						realTime: '0sec',
						type: 'Jumping',
						description: 'Classic high-energy cardiovascular exercise that improves full-body coordination and heart health.',
						steps: [
							'Stand tall with feet together, arms resting at your sides.',
							'Jump your feet out to the sides while raising arms overhead.',
							'Immediately jump back to the starting position.',
							'Repeat in a fluid, rhythmic motion.'
						],
						image: 'https://images.unsplash.com/photo-1476480862126-209bfaa8edc8?w=800&auto=format&fit=crop&q=60'
					}
				);
			} else if (day === 2) {
				exercises.push(
					{
						id: `ex-${day}-1`,
						name: 'Push-ups',
						duration: '8 mins',
						calories: '60Kcal',
						status: 'todo',
						difficulty: '3',
						totalTime: '45sec',
						realTime: '0sec',
						type: 'Upper Body',
						description: 'A fundamental bodyweight movement that strengthens the chest, shoulders, triceps, and core.',
						steps: [
							'Start in a high plank position, hands slightly wider than shoulders.',
							'Lower your chest toward the floor, keeping elbows at a 45-degree angle.',
							'Push back up to the starting position, keeping your core tight.',
							'Maintain a straight line from head to toe.'
						],
						image: 'https://images.unsplash.com/photo-1571019613454-1cb2f99b2d8b?w=800&auto=format&fit=crop&q=60'
					},
					{
						id: `ex-${day}-2`,
						name: 'Bodyweight Squats',
						duration: '10 mins',
						calories: '70Kcal',
						status: 'todo',
						difficulty: '2',
						totalTime: '45sec',
						realTime: '0sec',
						type: 'Legs',
						description: 'Strengthens the quadriceps, hamstrings, and glutes while improving hip mobility.',
						steps: [
							'Stand with feet shoulder-width apart, toes pointing forward or slightly out.',
							'Lower your hips back and down, as if sitting in a chair.',
							'Keep your chest proud and knees tracking over toes.',
							'Drive through heels to return to standing.'
						],
						image: 'https://images.unsplash.com/photo-1583454110551-21f2fa2afe61?w=800&auto=format&fit=crop&q=60'
					},
					{
						id: `ex-${day}-3`,
						name: 'Plank Hold',
						duration: '8 mins',
						calories: '30Kcal',
						status: 'todo',
						difficulty: '2',
						totalTime: '60sec',
						realTime: '0sec',
						type: 'Core',
						description: 'Isometric core exercise that improves spinal stability, shoulder strength, and posture.',
						steps: [
							'Place elbows directly under shoulders, forearms flat on the floor.',
							'Extend legs behind you, rising onto toes.',
							'Engage abs and glutes, keeping back flat.',
							'Hold and breathe steadily.'
						],
						image: 'https://images.unsplash.com/photo-1571019613454-1cb2f99b2d8b?w=800&auto=format&fit=crop&q=60'
					}
				);
			} else {
				exercises.push(
					{
						id: `ex-${day}-1`,
						name: 'Mountain Climbers',
						duration: '10 mins',
						calories: '85Kcal',
						status: 'todo',
						difficulty: '3',
						totalTime: '40sec',
						realTime: '0sec',
						type: 'Cardio',
						description: 'High-intensity core and cardio movement that builds explosive leg strength and increases stamina.',
						steps: [
							'Start in a push-up position, hands under shoulders.',
							'Drive your right knee up toward your chest.',
							'Switch legs quickly, drawing the left knee in while extending the right leg.',
							'Keep hips down and run in place.'
						],
						image: 'https://images.unsplash.com/photo-1476480862126-209bfaa8edc8?w=800&auto=format&fit=crop&q=60'
					},
					{
						id: `ex-${day}-2`,
						name: 'Dumbbell Bicep Curls',
						duration: '12 mins',
						calories: '45Kcal',
						status: 'todo',
						difficulty: '1',
						totalTime: '30sec',
						realTime: '0sec',
						type: 'Arms',
						description: 'Isolates the biceps to build arm strength and definition.',
						steps: [
							'Stand tall with a dumbbell in each hand, palms facing forward.',
							'Squeeze biceps and bend elbows to lift weights to shoulder height.',
							'Lower dumbbells slowly to return to the start.',
							'Avoid swinging your body.'
						],
						image: 'https://images.unsplash.com/photo-1583454110551-21f2fa2afe61?w=800&auto=format&fit=crop&q=60'
					}
				);
			}
		}
		plans.push({ dayNumber: day, exercises });
	}
	return plans;
}
