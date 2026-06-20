import { Directive, HostListener, Input } from '@angular/core';

@Directive({
	selector: 'img[fallbackSrc]',
	standalone: true,
})
export class FallbackSrcDirective {
	@Input() fallbackSrc = '/assets/fitness_logo.jpg';

	@HostListener('error', ['$event'])
	onError(event: Event) {
		const img = event.target as HTMLImageElement;
		if (img.src.endsWith(this.fallbackSrc)) return;
		img.src = this.fallbackSrc;
	}
}
