import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-course-search',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './course-search.component.html',
  styleUrl: './course-search.component.scss'
})
export class CourseSearchComponent {
	onSubmit() {
		console.log(123);
	}
}
