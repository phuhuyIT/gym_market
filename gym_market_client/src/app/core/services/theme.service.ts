import { Injectable, signal } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class ThemeService {
  private _dark = signal(false);
  readonly isDark = this._dark.asReadonly();

  init(): void {
    const saved = localStorage.getItem('gymmarket-theme');
    const prefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
    this._dark.set(saved ? saved === 'dark' : prefersDark);
    this.apply();
  }

  toggle(): void {
    this._dark.update(d => !d);
    this.apply();
    localStorage.setItem('gymmarket-theme', this._dark() ? 'dark' : 'light');
  }

  private apply(): void {
    document.documentElement.setAttribute('data-theme', this._dark() ? 'dark' : 'light');
  }
}
