import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { PagesClientComponent } from './pages-client.component';
import { YourProfileComponent } from './your-profile/your-profile.component';
import { TrainerListComponent } from './trainer-list/trainer-list.component';
import { TrainerDetailsComponent } from './trainer-details/trainer-details.component';
import { CourseSearchComponent } from './course-search/course-search.component';
import { CourseDetailsComponent } from './course-details/course-details.component';
import { UpdateProfileComponent } from './update-profile/update-profile.component';
import { CourseRegistrationComponent } from './course-registration/course-registration.component';
import { FoodNutritionCalculatorComponent } from './food-nutrition-calculator/food-nutrition-calculator.component';
import { AccountSettingsComponent } from '../account-settings/account-settings.component';

export const routes: Routes = [
	{
		path: '',
        component: PagesClientComponent,
		children: [
			{ path: '', redirectTo: 'home-client', pathMatch: 'full' },
			{ path: 'home-client', component: HomeComponent, title: 'Home' },
			{ path: 'your-profile', component: YourProfileComponent, title: 'Your profile' },
			{ path: 'update-profile', component: UpdateProfileComponent, title: 'Update profile' },
			{ path: 'account-settings', component: AccountSettingsComponent, title: 'Account Settings' },
			{ path: 'find-trainer', component: TrainerListComponent, title: 'Find trainer' },
			{ path: 'trainer-details/:id', component: TrainerDetailsComponent, title: 'Trainer details' },
			{ path: 'course-search', component: CourseSearchComponent, title: 'Course search' },
			{ path: 'course-details/:id', component: CourseDetailsComponent, title: 'Course details' },
			{ path: 'course-registration', component: CourseRegistrationComponent, title: 'Course Registration' },
			{ path: 'food-nutrition-calculator', component: FoodNutritionCalculatorComponent, title: 'Food Nutrition Calculator' },
		],
	},
];
