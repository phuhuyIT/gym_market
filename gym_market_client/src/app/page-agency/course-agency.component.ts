import { Component, ChangeDetectionStrategy } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { HeaderComponent } from '../shared/components/header/header.component';
import { FooterComponent } from '../shared/components/footer/footer.component';

@Component({
    selector: 'app-course-agency',
    changeDetection: ChangeDetectionStrategy.OnPush,
    imports: [RouterOutlet, HeaderComponent, FooterComponent],
    templateUrl: './course-agency.component.html',
    styleUrl: './course-agency.component.scss'
})
export class CourseAgencyComponent {
}

