# IncidentInsight

IncidentInsight is a solution designed to fetch and summarize incident details, providing concise and actionable insights for incident management and resolution.

## Features

- **Fetch Incident Details**: Fetches incident details from different incident management system. e.g ADO , JIRA
- **Summarize Incident Details**: Utilizes advanced natural language processing techniques to summarize incident details into concise and informative summaries.
- **Easy Integration**: Simple integration with existing incident management workflows or tools.
- **Customizable**: Flexible configuration options allow customization based on specific needs and preferences.
- **Scalable**: Designed to handle large volumes of incident data efficiently, suitable for organizations of all sizes.
- **Insightful Reports**: Generates insightful reports and summaries for incident analysis and trend identification.

## Getting Started

### Installation

1. Clone the repository: `git clone https://github.com/SRIVASTAVAABHISHEKGAURAV/IncidentInsight.git`
2. Install dependencies: `Nuget Packages`

### Configuration

Edit the `config.json` file to customize settings such as API keys, data sources, and summarization parameters.

```json
{
 "Secrets": {
    "Environment": "JIRA",
    "PATToken": "",
    "ApiToken": "",
    "UserName": "",
    "JiraBaseUrl": "",
    "Uri": "",
    "Project": "",
    "AoaiDeploymentName": "",
    "AoaiEndpoint": "",
    "AoaiKey": "",
    "Prompt": "",
    "MaxTokens": 300
  }
}
```

## Contributing

Contributions are welcome! Please follow the [Contribution Guidelines](CONTRIBUTING.md) to get started.

## License

This project is licensed under the [MIT License](LICENSE).

## Contact

For any inquiries or support, please contact us at developer.abhishekgaurav@gmail.com.
