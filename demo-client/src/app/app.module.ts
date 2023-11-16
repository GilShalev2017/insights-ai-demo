import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppComponent } from './app.component';
import { ToolbarComponent } from './toolbar/toolbar.component';
import { VideoGridComponent } from './video-grid/video-grid.component';

import { MatMenuModule } from '@angular/material/menu';
import { MatSidenavModule } from '@angular/material/sidenav'
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatListModule } from '@angular/material/list';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatGridListModule } from '@angular/material/grid-list';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MainMenuComponent } from './main-menu/main-menu.component';
import { FlexLayoutModule } from '@angular/flex-layout';
import { VideoSourceComponent } from './video-source/video-source.component';
import { MatDividerModule } from '@angular/material/divider';
import { FormsModule } from '@angular/forms';
import { MatDialogModule} from '@angular/material/dialog';
import { VideoSourceListComponent } from './video-source-list/video-source-list.component';
import { MatCardModule} from '@angular/material/card';
import { SubjectComponent } from './subject/subject.component';
import { SubjectListComponent } from './subject-list/subject-list.component';
import { SelectVideoComponent } from './select-video/select-video.component';
import { VideoCellComponent } from './video-cell/video-cell.component';
import { HttpClientModule } from '@angular/common/http';
import { MatExpansionModule } from '@angular/material/expansion';
import {MatDatepickerModule} from '@angular/material/datepicker';
import {MatNativeDateModule} from '@angular/material/core';
import { MatInputModule } from '@angular/material/input';
import { TabbedLayoutComponent } from './tabbed-layout/tabbed-layout.component';
import { MatTabsModule } from '@angular/material/tabs';
import { AzureVideoPanelComponent } from './azure-video-panel/azure-video-panel.component';
import { AzureLayoutComponent } from './azure-layout/azure-layout.component';
import { PeopleComponent } from './people/people.component';
import { LabelsComponent } from './labels/labels.component';
import { MatChipsModule } from '@angular/material/chips';
import { AudioEffectComponent } from './audio-effect/audio-effect.component';
import { NamedEntitiesComponent } from './named-entities/named-entities.component';
import { EmotionsComponent } from './emotions/emotions.component';
import { TimeLineComponent } from './time-line/time-line.component';
import { MatSliderModule } from '@angular/material/slider';
import { SummaryComponent } from './summary/summary.component';
import { SentimentComponent } from './sentiment/sentiment.component';
import { KeyPointsComponent } from './key-points/key-points.component';
import { ActionItemsComponent } from './action-items/action-items.component';
import { SearchBarComponent } from './search-bar/search-bar.component';
import { TranscriptionsComponent } from './transcriptions/transcriptions.component';
import { LanguagesMenuComponent } from './languages-menu/languages-menu.component';
import { VideoDetailsComponent } from './video-details/video-details.component';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { InsightsContainerComponent } from './insights-container/insights-container.component';
import { TimeSpanPipe } from './pipes/time-span.pipe';

import { AngularSplitModule } from 'angular-split';
import { SplitExampleComponent } from './split-example/split-example.component';

@NgModule({
  declarations: [
    AppComponent,
    ToolbarComponent,
    VideoGridComponent,
    MainMenuComponent,
    VideoSourceComponent,
    VideoSourceListComponent,
    SubjectComponent,
    SubjectListComponent,
    SelectVideoComponent,
    VideoCellComponent,
    TabbedLayoutComponent,
    AzureVideoPanelComponent,
    AzureLayoutComponent,
    PeopleComponent,
    LabelsComponent,
    AudioEffectComponent,
    NamedEntitiesComponent,
    EmotionsComponent,
    TimeLineComponent,
    SummaryComponent,
    SentimentComponent,
    KeyPointsComponent,
    ActionItemsComponent,
    SearchBarComponent,
    TranscriptionsComponent,
    LanguagesMenuComponent,
    VideoDetailsComponent,
    InsightsContainerComponent,
    TimeSpanPipe,
    SplitExampleComponent
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    FormsModule,
    HttpClientModule,
    MatDialogModule,
    MatToolbarModule,
    MatSidenavModule,
    MatListModule,
    MatButtonModule,
    MatMenuModule,
    MatIconModule,
    MatFormFieldModule,
    MatSelectModule,
    MatDividerModule,
    MatGridListModule,
    FlexLayoutModule,
    MatCardModule,
    MatExpansionModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatInputModule,
    MatTabsModule,
    MatChipsModule,
    MatSliderModule,
    MatProgressSpinnerModule,
    AngularSplitModule
  ],
  providers: [],
  bootstrap: [TabbedLayoutComponent]
})
export class AppModule { }
