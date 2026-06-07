import { Injectable, signal } from '@angular/core';
import { STORAGE_KEYS } from '../../utilities/storage-keys.const';

@Injectable({ providedIn: 'root' })
export class ThemeService {
  private _dark = signal(false);
  readonly isDark = this._dark.asReadonly();

  init(): void {
    const saved = localStorage.getItem(STORAGE_KEYS.theme);
    const prefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
    this._dark.set(saved ? saved === 'dark' : prefersDark);
    this.apply();
  }

  toggle(): void {
    this._dark.update(d => !d);
    this.apply();
    localStorage.setItem(STORAGE_KEYS.theme, this._dark() ? 'dark' : 'light');
  }

  private apply(): void {
    document.documentElement.setAttribute('data-theme', this._dark() ? 'dark' : 'light');
  }
}
