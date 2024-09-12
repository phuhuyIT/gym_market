import { ApplicationConfig } from '@angular/core';
import {
	provideRouter,
	withComponentInputBinding,
	withInMemoryScrolling,
	withViewTransitions,
} from '@angular/router';

import { routes } from './app.routes';
import { provideHttpClient } from '@angular/common/http';

export const appConfig: ApplicationConfig = {
	providers: [
		provideRouter(
			routes,
			withViewTransitions(),
			withComponentInputBinding(),
			withInMemoryScrolling({ scrollPositionRestoration: 'top', anchorScrolling: 'enabled' })
		),
		provideHttpClient(),
	],
};
