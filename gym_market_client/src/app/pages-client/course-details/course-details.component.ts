import { Component } from '@angular/core';

@Component({
	selector: 'app-course-details',
	standalone: true,
	imports: [],
	templateUrl: './course-details.component.html',
	styleUrl: './course-details.component.scss',
})
export class CourseDetailsComponent {
	courseOptions: any;
	constructor() {}

	ngOnInit() {
		this.courseOptions = [
			{
				optionId: '43fbd34c-5620-4e08-b9d0-98f95e3dedcd',
				optionName: 'Omnis amet esse debitis suscipit suscipit voluptatem.',
				description:
					'Quibusdam earum facilis omnis qui iure pariatur consequatur laboriosam sequi. Assumenda quia consequatur quaerat sit voluptatem soluta corporis dignissimos. Eum sunt nihil molestiae sint occaecati quo illo et. Voluptas nesciunt exercitationem libero. Voluptas sunt et suscipit cupiditate eligendi. Distinctio aspernatur rem sunt rerum ut.',
				price: 1000000.0,
			},
			{
				optionId: 'a2d9acaa-74d7-455b-b52f-4d69618a3d6a',
				optionName: 'A harum tempore magni et reprehenderit.',
				description:
					'Velit laboriosam accusantium dolore est. Ipsam molestiae nostrum voluptates sed laborum. Vitae veniam itaque magnam at molestiae. Quisquam dignissimos quod quibusdam dolorum et. Consequatur aut eos officiis et. Ad dicta molestias beatae veniam quos mollitia eum dolores.\n \nAtque itaque qui sapiente. Nihil nemo molestiae omnis. Praesentium quis ullam placeat. Accusamus quia eos numquam asperiores voluptatem autem magni.\n \n',
				price: 400.0,
			},
			{
				optionId: 'ac92f682-5559-4909-8b29-543ab81a178e',
				optionName: 'Qui expedita nesciunt sapiente voluptatem ut illo.',
				description:
					'Cupiditate nihil autem sunt sed ex iure fuga dolores quis. Quis et accusantium vero voluptates consequuntur. Cumque repellat cum reiciendis. Libero incidunt dignissimos. Animi esse voluptatem est reiciendis aut sunt est libero.',
				price: 4.0,
			},
		];
	}
}
