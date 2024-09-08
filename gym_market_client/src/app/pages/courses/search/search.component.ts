import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-search',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './search.component.html',
  styleUrl: './search.component.scss'
})
export class SearchComponent {
  courses: any = [];

  ngOnInit() {
    this.courses = [
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
      {
        image: 'https://cdn01.alison-static.net/courses/5482/alison_courseware_intro_5482.jpg',
        name: 'Amet sint omnis dolorum rem ut exercitationem maiores unde.',
        hour: '1 - 1 hours',
        learners: '23423',
      },
      {
        image: 'https://cdn01.alison-static.net/courses/5482/alison_courseware_intro_5482.jpg',
        name: 'Ut eos eius alias repellendus velit aut nobis.',
        hour: '2 - 6 hours',
        learners: '42345',
      },
      {
        image: 'https://cdn01.alison-static.net/courses/1358/alison_courseware_intro_1358.jpg',
        name: 'Praesentium aspernatur consequatur.',
        hour: '2 - 6 hours',
        learners: '2344',
      },
      {
        image: 'https://cdn01.alison-static.net/courses/1358/alison_courseware_intro_1358.jpg',
        name: 'Accusamus sint distinctio molestias.',
        hour: '2 - 6 hours',
        learners: '54534',
      },
      {
        image: 'https://cdn01.alison-static.net/courses/6183/alison_courseware_intro_6183.jpg',
        name: 'Ipsam alias iure cupiditate adipisci earum pariatur dolor totam.',
        hour: '2 - 6 hours',
        learners: '23432',
      },
      {
        image: 'https://cdn01.alison-static.net/courses/6183/alison_courseware_intro_6183.jpg',
        name: 'Ea et veritatis velit.',
        hour: '2 - 6 hours',
        learners: '35543',
      },
      {
        image: 'https://cdn01.alison-static.net/courses/1690/alison_courseware_intro_1690.jpg',
        name: 'Voluptatem sunt quos voluptatem iure qui vel veritatis.',
        hour: '2 - 6 hours',
        learners: '234234',
      },
      {
        image: 'https://cdn01.alison-static.net/courses/1690/alison_courseware_intro_1690.jpg',
        name: 'Totam voluptatem et nulla eaque fugit aut ducimus.',
        hour: '2 - 6 hours',
        learners: '242342',
      },
      {
        image: 'https://cdn01.alison-static.net/courses/5484/alison_courseware_intro_5484.jpg',
        name: 'Et quae molestiae animi sed maiores omnis est quae.',
        hour: '2 - 6 hours',
        learners: '42423',
      },
      {
        image: 'https://cdn01.alison-static.net/courses/5484/alison_courseware_intro_5484.jpg',
        name: 'Ullam nobis at deleniti facere doloribus quibusdam.',
        hour: '2 - 6 hours',
        learners: '42432',
      },
      {
        image: 'https://cdn01.alison-static.net/courses/1791/alison_courseware_intro_1791.jpg',
        name: 'Sapiente incidunt aut ut dolorum ut fuga molestiae ut sint.',
        hour: '2 - 6 hours',
        learners: '6534',
      },
      {
        image: 'https://cdn01.alison-static.net/courses/1791/alison_courseware_intro_1791.jpg',
        name: 'Temporibus non omnis.',
        hour: '2 - 6 hours',
        learners: '234234',
      },
    ]
  }

  onSubmit() {
    console.log(123);
    
  }

}
