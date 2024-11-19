import { Component } from '@angular/core';
import { HeaderComponent } from '../../components/header/header.component';
import { FooterComponent } from '../../components/footer/footer.component';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
	selector: 'app-course-details',
	standalone: true,
	imports: [HeaderComponent, FooterComponent, FormsModule, CommonModule],
	templateUrl: './course-details.component.html',
	styleUrl: './course-details.component.scss',
})
export class CourseDetailsComponent {
	cards: any;
  moreCourse: any

	ngOnInit() {
		this.cards = [
			{ title: 'Card 1', content: 'Nội dung của card 1' },
			{ title: 'Card 2', content: 'Nội dung của card 2' },
			{ title: 'Card 3', content: 'Nội dung của card 3' },
		];

		this.cards = this.cards.map((c: any) => ({ ...c, isExpanded: false, select: false }));
		
    this.moreCourse = [
      {
        image: 'https://cdn01.alison-static.net/courses/6143/alison_courseware_intro_6143.jpg',
        name: 'Aut impedit non voluptatem eos in dolor.',
        hour: '2 - 8 hours',
        learners: '6542',
      },
      {
        image: 'https://cdn01.alison-static.net/courses/6143/alison_courseware_intro_6143.jpg',
        name: 'Consequatur ipsum inventore non sint non nihil.',
        hour: '4 - 5 hours',
        learners: '25465',
      },
      {
        image: 'https://cdn01.alison-static.net/courses/1793/alison_courseware_intro_1793.jpg',
        name: 'Ut odio commodi qui sapiente.',
        hour: '1 - 2 hours',
        learners: '34234',
      },
      {
        image: 'https://cdn01.alison-static.net/courses/1793/alison_courseware_intro_1793.jpg',
        name: 'Iure esse eos asperiores praesentium.',
        hour: '2 - 4 hours',
        learners: '23465',
      },
    ]
	}

	onExpandItem(item: any) {
		item.isExpanded = !item.isExpanded;
	}

	onSelect(item: any) {
		item.select = !item.select;
	}
}
