import { FinishedGameDto } from '../models/game/FinishedGameDto'

const GameSummaryTable = ({ finishedGameData }: { finishedGameData: FinishedGameDto }) => {
	return (
		<table>
			<thead>
				<tr>
					<th>Round</th>
					<th>Score</th>
				</tr>
			</thead>
			<tbody>
				{finishedGameData.rounds.map((round, index) => {
					return (
						<tr key={index}>
							<td>{index + 1}</td>
							<td>{round.score.toFixed(2)}</td>
						</tr>
					)
				})}
			</tbody>
		</table>
	)
}

export default GameSummaryTable
