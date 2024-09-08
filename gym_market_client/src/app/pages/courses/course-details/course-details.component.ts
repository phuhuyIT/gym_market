import { Component } from '@angular/core';
import { HeaderComponent } from "../../components/header/header.component";

@Component({
  selector: 'app-course-details',
  standalone: true,
  imports: [HeaderComponent],
  templateUrl: './course-details.component.html',
  styleUrl: './course-details.component.scss'
})
export class CourseDetailsComponent {
  year = 0;

  ngOnInit() {
    this.year = new Date().getFullYear();
  }
}
