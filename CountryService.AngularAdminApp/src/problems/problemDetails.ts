export interface ProblemDetails {
    status: number;
    type: string;
    title: string;
    detail: string;
    instance: string;
    requestId: string;
    correlationId: string;
    errors: Error[];
}
