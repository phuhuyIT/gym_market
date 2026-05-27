import { ApplicationConfig } from '@angular/core';
import {
	provideRouter,
	withComponentInputBinding,
	withInMemoryScrolling,
	withViewTransitions,
} from '@angular/router';

import { routes } from './app.routes';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { authInterceptor } from './core/interceptors/auth.interceptor';

export const appConfig: ApplicationConfig = {
	providers: [
		provideRouter(
			routes,
			withViewTransitions(),
			withComponentInputBinding(),
			withInMemoryScrolling({ scrollPositionRestoration: 'top', anchorScrolling: 'enabled' })
		),
		provideHttpClient(withInterceptors([authInterceptor])),
	],
};
