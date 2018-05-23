import { Component } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  title = 'app';

  toggleCollapse(event: MouseEvent) {
    (<HTMLElement>event.target).parentElement.classList.toggle('collapse');
  }
}
