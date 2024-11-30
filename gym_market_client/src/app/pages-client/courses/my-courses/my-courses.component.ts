import { Component, inject, Input } from '@angular/core';
import { AccountService } from '../../../account/account.service';
import { UserStore } from '../../../stores/user.store';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
	selector: 'app-my-courses',
	standalone: true,
	imports: [CommonModule, RouterLink],
	templateUrl: './my-courses.component.html',
	styleUrl: './my-courses.component.scss',
})
export class MyCoursesComponent {
	showAccountOption = false;
	showOther = false;
	userStore = inject(UserStore);
	private courses: any = [];
	courseViews: any = [];
	@Input() type: string | null = null;

	constructor(
		private accountService: AccountService,
		private router: Router,
		private route: ActivatedRoute
	) {}

	ngOnInit() {
		this.courses = [
			{
				id: 1,
				image: 'https://cdn01.alison-static.net/courses/6143/alison_courseware_intro_6143.jpg',
				name: 'Aut impedit non voluptatem eos in dolor.',
				hour: '2 - 8 hours',
				learners: '6542',
				type: 'yoga',
			},
			{
				id: 2,
				image: 'https://cdn01.alison-static.net/courses/6143/alison_courseware_intro_6143.jpg',
				name: 'Consequatur ipsum inventore non sint non nihil.',
				hour: '4 - 5 hours',
				learners: '25465',
				type: 'cardio',
			},
			{
				id: 3,
				image: 'https://cdn01.alison-static.net/courses/1793/alison_courseware_intro_1793.jpg',
				name: 'Ut odio commodi qui sapiente.',
				hour: '1 - 2 hours',
				learners: '34234',
				type: 'pilates',
			},
			{
				id: 4,
				image: 'https://cdn01.alison-static.net/courses/1793/alison_courseware_intro_1793.jpg',
				name: 'Iure esse eos asperiores praesentium.',
				hour: '2 - 4 hours',
				learners: '23465',
				type: 'yoga',
			},
			{
				id: 5,
				image: 'https://cdn01.alison-static.net/courses/5482/alison_courseware_intro_5482.jpg',
				name: 'Amet sint omnis dolorum rem ut exercitationem maiores unde.',
				hour: '1 - 1 hours',
				learners: '23423',
				type: 'cardio',
			},
			{
				id: 6,
				image: 'https://cdn01.alison-static.net/courses/5482/alison_courseware_intro_5482.jpg',
				name: 'Ut eos eius alias repellendus velit aut nobis.',
				hour: '2 - 6 hours',
				learners: '42345',
				type: 'yoga',
			},
			{
				id: 6,
				image: 'https://cdn01.alison-static.net/courses/1358/alison_courseware_intro_1358.jpg',
				name: 'Praesentium aspernatur consequatur.',
				hour: '2 - 6 hours',
				learners: '2344',
				type: 'cardio',
			},
			{
				id: 7,
				image: 'https://cdn01.alison-static.net/courses/1358/alison_courseware_intro_1358.jpg',
				name: 'Accusamus sint distinctio molestias.',
				hour: '2 - 6 hours',
				learners: '54534',
				type: 'yoga',
			},
			{
				id: 8,
				image: 'https://cdn01.alison-static.net/courses/6183/alison_courseware_intro_6183.jpg',
				name: 'Ipsam alias iure cupiditate adipisci earum pariatur dolor totam.',
				hour: '2 - 6 hours',
				learners: '23432',
				type: 'pilates',
			},
			{
				id: 9,
				image: 'https://cdn01.alison-static.net/courses/6183/alison_courseware_intro_6183.jpg',
				name: 'Ea et veritatis velit.',
				hour: '2 - 6 hours',
				learners: '35543',
				type: 'yoga',
			},
			{
				id: 10,
				image: 'https://cdn01.alison-static.net/courses/1690/alison_courseware_intro_1690.jpg',
				name: 'Voluptatem sunt quos voluptatem iure qui vel veritatis.',
				hour: '2 - 6 hours',
				learners: '234234',
				type: 'cardio',
			},
			{
				id: 11,
				image: 'https://cdn01.alison-static.net/courses/1690/alison_courseware_intro_1690.jpg',
				name: 'Totam voluptatem et nulla eaque fugit aut ducimus.',
				hour: '2 - 6 hours',
				learners: '242342',
				type: 'pilates',
			},
			{
				id: 12,
				image: 'https://cdn01.alison-static.net/courses/5484/alison_courseware_intro_5484.jpg',
				name: 'Et quae molestiae animi sed maiores omnis est quae.',
				hour: '2 - 6 hours',
				learners: '42423',
				type: 'pilates',
			},
			{
				id: 13,
				image: 'https://cdn01.alison-static.net/courses/5484/alison_courseware_intro_5484.jpg',
				name: 'Ullam nobis at deleniti facere doloribus quibusdam.',
				hour: '2 - 6 hours',
				learners: '42432',
				type: 'stretching',
			},
			{
				id: 14,
				image: 'https://cdn01.alison-static.net/courses/1791/alison_courseware_intro_1791.jpg',
				name: 'Sapiente incidunt aut ut dolorum ut fuga molestiae ut sint.',
				hour: '2 - 6 hours',
				learners: '6534',
				type: 'stretching',
			},
			{
				id: 15,
				image: 'https://cdn01.alison-static.net/courses/1791/alison_courseware_intro_1791.jpg',
				name: 'Temporibus non omnis.',
				hour: '2 - 6 hours',
				learners: '234234',
				type: 'cardio',
			},
			{
				id: 16,
				image: 'https://cdn01.alison-static.net/courses/6143/alison_courseware_intro_6143.jpg',
				name: 'Aut impedit non voluptatem eos in dolor.',
				hour: '2 - 8 hours',
				learners: '6542',
				type: 'aerobic',
			},
			{
				id: 17,
				image: 'https://cdn01.alison-static.net/courses/6143/alison_courseware_intro_6143.jpg',
				name: 'Consequatur ipsum inventore non sint non nihil.',
				hour: '4 - 5 hours',
				learners: '25465',
				type: 'aerobic',
			},
			{
				id: 18,
				image: 'https://cdn01.alison-static.net/courses/1793/alison_courseware_intro_1793.jpg',
				name: 'Ut odio commodi qui sapiente.',
				hour: '1 - 2 hours',
				learners: '34234',
				type: 'pilates',
			},
			{
				id: 19,
				image: 'https://cdn01.alison-static.net/courses/1793/alison_courseware_intro_1793.jpg',
				name: 'Iure esse eos asperiores praesentium.',
				hour: '2 - 4 hours',
				learners: '23465',
				type: 'aerobic',
			},
			{
				id: 20,
				image: 'https://cdn01.alison-static.net/courses/5482/alison_courseware_intro_5482.jpg',
				name: 'Amet sint omnis dolorum rem ut exercitationem maiores unde.',
				hour: '1 - 1 hours',
				learners: '23423',
				type: 'aerobic',
			},
			{
				id: 21,
				image: 'https://cdn01.alison-static.net/courses/5482/alison_courseware_intro_5482.jpg',
				name: 'Ut eos eius alias repellendus velit aut nobis.',
				hour: '2 - 6 hours',
				learners: '42345',
				type: 'yoga',
			},
			{
				id: 22,
				image: 'https://cdn01.alison-static.net/courses/1358/alison_courseware_intro_1358.jpg',
				name: 'Praesentium aspernatur consequatur.',
				hour: '2 - 6 hours',
				learners: '2344',
				type: 'cardio',
			},
			{
				id: 23,
				image: 'https://cdn01.alison-static.net/courses/1358/alison_courseware_intro_1358.jpg',
				name: 'Accusamus sint distinctio molestias.',
				hour: '2 - 6 hours',
				learners: '54534',
				type: 'yoga',
			},
			{
				id: 24,
				image: 'https://cdn01.alison-static.net/courses/6183/alison_courseware_intro_6183.jpg',
				name: 'Ipsam alias iure cupiditate adipisci earum pariatur dolor totam.',
				hour: '2 - 6 hours',
				learners: '23432',
				type: 'crossfit',
			},
			{
				id: 25,
				image: 'https://cdn01.alison-static.net/courses/6183/alison_courseware_intro_6183.jpg',
				name: 'Ea et veritatis velit.',
				hour: '2 - 6 hours',
				learners: '35543',
				type: 'crossfit',
			},
			{
				id: 26,
				image: 'https://cdn01.alison-static.net/courses/1690/alison_courseware_intro_1690.jpg',
				name: 'Voluptatem sunt quos voluptatem iure qui vel veritatis.',
				hour: '2 - 6 hours',
				learners: '234234',
				type: 'cardio',
			},
			{
				id: 27,
				image: 'https://cdn01.alison-static.net/courses/1690/alison_courseware_intro_1690.jpg',
				name: 'Totam voluptatem et nulla eaque fugit aut ducimus.',
				hour: '2 - 6 hours',
				learners: '242342',
				type: 'crossfit',
			},
			{
				id: 28,
				image: 'https://cdn01.alison-static.net/courses/5484/alison_courseware_intro_5484.jpg',
				name: 'Et quae molestiae animi sed maiores omnis est quae.',
				hour: '2 - 6 hours',
				learners: '42423',
				type: 'crossfit',
			},
			{
				id: 29,
				image: 'https://cdn01.alison-static.net/courses/5484/alison_courseware_intro_5484.jpg',
				name: 'Ullam nobis at deleniti facere doloribus quibusdam.',
				hour: '2 - 6 hours',
				learners: '42432',
				type: 'stretching',
			},
			{
				id: 30,
				image: 'https://cdn01.alison-static.net/courses/1791/alison_courseware_intro_1791.jpg',
				name: 'Sapiente incidunt aut ut dolorum ut fuga molestiae ut sint.',
				hour: '2 - 6 hours',
				learners: '6534',
				type: 'stretching',
			},
			{
				id: 31,
				image: 'https://cdn01.alison-static.net/courses/1791/alison_courseware_intro_1791.jpg',
				name: 'Temporibus non omnis.',
				hour: '2 - 6 hours',
				learners: '234234',
				type: 'stretching',
			},
		];
		this.route.paramMap.subscribe(params => {
			this.type = params.get('type');

      if(this.type !== null && this.type !== 'all') {
        this.courseViews = this.courses.filter((c:any) => c.type === this.type);
      }
      else {
        this.courseViews = this.courses.filter((c:any) => c);
      }
		});
	}

	onShowAccountOption() {
		this.showAccountOption = true;
	}

	onHiddenAccountOption() {
		this.showAccountOption = false;
	}

	logout() {
		this.accountService.logout();
		this.router.navigateByUrl('/account/login');
	}

	onShowOther() {
		this.showOther = true;
	}

	onHiddenOther() {
		this.showOther = false;
	}
}
