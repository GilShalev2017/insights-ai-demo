export interface Transcript {
    [x: string]: any
    Text: string,
    StartInSeconds: string,
    EndInSeconds: string,
    isActive?: boolean,
    isPlaying?: boolean
    absoluteStartTime?: number
}
export interface InsightsResponse {
    Transcripts: Transcript[],
    OriginalTranscripts?: Transcript[],
    Summary: string,
    KeyPoints: string,
    ActionItems: string,
    Sentiment: string,
    KeyPointsList?: string[],
    ActionItemsList?: string[],
}